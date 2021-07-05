using Npgsql;
using StswExpress;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using WBZ.Models;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ
{
    internal static class SQL
    {
        internal static string connWBZ = null; /// przypisywanie połączenia w oknie logowania
		internal static NpgsqlConnection ConnOpenedWBZ => (NpgsqlConnection)StswExpress.SQL.OpenConnection(new NpgsqlConnection(connWBZ));

        #region Login
        /// <summary>
        /// Loguje użytkownika do systemu
        /// </summary>
        /// <param name="login">Nazwa użytkownika lub adres e-mail</param>
        /// <param name="password">Hasło do konta</param>
        /// <returns></returns>
        internal static bool Login(string login, string password)
        {
            bool result = false;
            var dt = new DataTable();

            using var sqlConn = ConnOpenedWBZ;
            var sqlCmd = new NpgsqlCommand(@"select * from wbz.users where login=@login and password=@password", sqlConn);
            sqlCmd.Parameters.AddWithValue("login", login);
            sqlCmd.Parameters.AddWithValue("password", password);
            using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
                sqlDA.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                Config.User = dt.ToList<M_User>()[0];
                /*if (Config.User.IsArchival)
                    new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Użytkownik o podanym loginie jest zarchiwizowany.").ShowDialog();
                else*/ if (Config.User.IsBlocked)
                    new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Użytkownik o podanym loginie jest zablokowany.").ShowDialog();
                else
                {
                    Config.User.Perms = GetUserPerms(Config.User.ID);
                    result = true;
                }
            }
            else new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Brak użytkownika w bazie lub złe dane logowania.").ShowDialog();

            return result;
        }

        /// <summary>
        /// Pobiera uprawnienia użytkownika
        /// </summary>
        /// <param name="userID">ID użytkownika</param>
        internal static List<string> GetUserPerms(int userID)
        {
            var result = new List<string>();
            var dt = new DataTable();

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var query = @"select perm from wbz.users_permissions where user_id=@user_id";
                using var sqlDA = new NpgsqlDataAdapter(query, sqlConn);
                sqlDA.SelectCommand.Parameters.AddWithValue("user_id", userID);
                sqlDA.Fill(dt);
                foreach (DataRow row in dt.Rows)
                    result.Add(row["perm"].ToString());
            }
            catch (Exception ex)
            {
                Error("Błąd pobierania uprawnień użytkownika", ex, Config.ListModules[0], userID);
            }

            return result;
        }

        /// <summary>
        /// Tworzy konto użytkownika/administratora w bazie o podanych parametrach
        /// </summary>
        /// <param name="email">Adres e-mail</param>
        /// <param name="login">Nazwa użytkownika</param>
        /// <param name="password">Hasło do konta</param>
        /// <param name="admin">Czy nadać uprawnienia administracyjne</param>
        /// <returns></returns>
        internal static bool Register(string email, string login, string password, bool admin = true)
        {
            using (var sqlConn = ConnOpenedWBZ)
            using (var sqlTran = sqlConn.BeginTransaction())
            {
                var sqlCmd = new NpgsqlCommand(@"insert into wbz.users (email, login, password)
				    values (@email, @login, @password) returning id", sqlConn, sqlTran);
                sqlCmd.Parameters.AddWithValue("email", email);
                sqlCmd.Parameters.AddWithValue("login", login);
                sqlCmd.Parameters.AddWithValue("password", Global.sha256(password));
                var id = Convert.ToInt32(sqlCmd.ExecuteScalar());

                /// permissions
                if (admin)
                {
                    sqlCmd = new NpgsqlCommand(@"insert into wbz.users_permissions (user_id, perm)
					    values (@user_id, 'Admin'), (@user_id, 'Users_PREVIEW'), (@user_id, 'Users_SAVE'), (@user_id, 'Users_DELETE')", sqlConn, sqlTran);
                    sqlCmd.Parameters.AddWithValue("user_id", id);
                    sqlCmd.ExecuteNonQuery();
                }

                sqlTran.Commit();
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Pobiera przelicznik głównej jednostki miary towaru
        /// </summary>
        /// <param name="articleID">ID towaru</param>
        /// <returns></returns>
        internal static double GetArtDefMeaCon(int articleID)
        {
            double result = 0;

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"select wbz.artdefmeacon(@article_id)", sqlConn);
                sqlCmd.Parameters.AddWithValue("article_id", articleID);
                result = Convert.ToDouble(sqlCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Error("Błąd pobierania przelicznika głównej jednostki miary towaru", ex, Config.GetModule(nameof(Modules.Articles)), articleID);
            }

            return result;
        }

        /// <summary>
        /// Ustawia stany towaru
        /// </summary>
        /// <param name="storeID">ID magazynu</param>
        /// <param name="articleID">ID towaru</param>
        /// <param name="quantity">Ilość towaru (kg)</param>
        /// <param name="measure">Jednostka miary</param>
        /// <param name="reserved">Czy rezerwacja</param>
        /// <param name="sqlConn">Połączenie SQL</param>
        /// <param name="sqlTran">Transakcja SQL</param>
        /// <returns></returns>
        internal static bool ChangeArticleQuantity(int storeID, int articleID, double quantity, string measure, bool reserved, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
        {
            bool result = false;

            try
            {
                var sqlCmd = new NpgsqlCommand(@"select case
					when exists(select from wbz.stores_articles where store_id=@store_id and article_id=@article_id) then true
					else false end", sqlConn, sqlTran);
                sqlCmd.Parameters.AddWithValue("store_id", storeID);
                sqlCmd.Parameters.AddWithValue("article_id", articleID);
                bool exists = Convert.ToBoolean(sqlCmd.ExecuteScalar());

                if (!reserved)
                {
                    if (exists)
                        sqlCmd = new NpgsqlCommand(@"update wbz.stores_articles
							set quantity=quantity+(@quantity * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1))
							where store=@store and article=@article", sqlConn, sqlTran);
                    else
                        sqlCmd = new NpgsqlCommand(@"insert into wbz.stores_articles (store, article, quantity, reserved)
							values (@store, @article, (@quantity * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1)), 0)", sqlConn, sqlTran);
                }
                else
                {
                    if (exists)
                        sqlCmd = new NpgsqlCommand(@"update wbz.stores_articles
							set reserved=reserved+(@quantity * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1))
							where store=@store and article=@article", sqlConn, sqlTran);
                    else
                        sqlCmd = new NpgsqlCommand(@"insert into wbz.stores_articles (store, article, quantity, reserved)
							values (@store, @article, 0, (@quantity * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1)))", sqlConn, sqlTran);
                }
                sqlCmd.Parameters.AddWithValue("store", storeID);
                sqlCmd.Parameters.AddWithValue("article", articleID);
                sqlCmd.Parameters.AddWithValue("quantity", quantity);
                sqlCmd.Parameters.AddWithValue("measure", measure);
                sqlCmd.ExecuteNonQuery();

                result = true;
            }
            catch (Exception ex)
            {
                Error("Błąd zmiany ilości towaru na stanie", ex, Config.GetModule(nameof(Modules.Articles)), articleID);
            }

            return result;
        }

        /// <summary>
        /// Pobiera sformatowaną tabelę dla pozycji dystrybucji
        /// </summary>
        internal static DataTable GetDistributionPositionsFormatting()
        {
            var result = new DataTable();

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"select 0 as id, 0 as pos, 0 as store_id, '' as store_name,
					0 as article_id, '' as article_name, 0.0 as quantity, '' as measure", sqlConn);
                using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                sqlDA.Fill(result);
                result.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Pobiera dane o pozycjach dystrybucji
        /// </summary>
        /// <param name="id">ID instancji</param>
        internal static List<M_DistributionFamily> GetDistributionPositions(int id)
        {
            var result = new List<M_DistributionFamily>();
            var dt = new DataTable();

            try
            {
                using (var sqlConn = ConnOpenedWBZ)
                {
                    NpgsqlCommand sqlCmd;

                    ///add
                    if (id == 0)
                        dt = GetDistributionPositionsFormatting();
                    ///edit
                    else
                    {
                        sqlCmd = new NpgsqlCommand(@"select id, pos, family_id, (select lastname from wbz.families where id=dp.family_id) as family_name, members,
								store_id, (select name from wbz.stores where id=dp.store_id) as store_name,
								article_id, (select name from wbz.articles where id=dp.article_id) as article_name,
								quantity / wbz.ArtDefMeaCon(dp.article_id) as quantity, coalesce(nullif(wbz.ArtDefMeaNam(dp.article_id),''), 'kg') as measure, status
							from wbz.distributions_positions dp
							where distribution=@id", sqlConn);
                        sqlCmd.Parameters.AddWithValue("id", id);
                        using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                        sqlDA.Fill(dt);
                    }
                }

                ///przypisywanie towarów do rodzin
                foreach (DataRow row in dt.Rows)
                {
                    var family = result.FirstOrDefault(x => x.FamilyID == (int)row["family"]);
                    if (family == null)
                    {
                        family = new M_DistributionFamily()
                        {
                            FamilyID = (int)row["family_id"],
                            FamilyName = (string)row["family_name"],
                            Members = Convert.ToInt16(row["members"]),
                            Status = Convert.ToInt16(row["status"])
                        };
                        result.Add(family);
                        family = result.FirstOrDefault(x => x.FamilyID == (int)row["family_id"]);
                    }

                    var position = family.Positions.NewRow();

                    position["id"] = row["id"];
                    position["pos"] = row["pos"];
                    position["store_id"] = row["store_id"];
                    position["store_name"] = row["store_name"];
                    position["article_id"] = row["article_id"];
                    position["article_name"] = row["article_name"];
                    position["quantity"] = row["quantity"];
                    position["measure"] = row["measure"];

                    family.Positions.Rows.Add(position);
                    family.Positions.Rows[^1].AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        #region module "Stats"
        /// <summary>
        /// Pobiera ilości towarów wg lat i miesięcy
        /// </summary>
        internal static DataTable GetStatsArticles()
        {
            var result = new DataTable();

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"select year,
						sum(m_01) as month_01,
						sum(m_02) as month_02,
						sum(m_03) as month_03,
						sum(m_04) as month_04,
						sum(m_05) as month_05,
						sum(m_06) as month_06,
						sum(m_07) as month_07,
						sum(m_08) as month_08,
						sum(m_09) as month_09,
						sum(m_10) as month_10,
						sum(m_11) as month_11,
						sum(m_12) as month_12
					from
					(
						select extract(year from dateissue) as year,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  1) as m_01,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  2) as m_02,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  3) as m_03,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  4) as m_04,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  5) as m_05,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  6) as m_06,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  7) as m_07,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  8) as m_08,
							(SELECT sum(quantity) WHERE extract(month from dateissue) =  9) as m_09,
							(SELECT sum(quantity) WHERE extract(month from dateissue) = 10) as m_10,
							(SELECT sum(quantity) WHERE extract(month from dateissue) = 11) as m_11,
							(SELECT sum(quantity) WHERE extract(month from dateissue) = 12) as m_12
						from wbz.documents i
						inner join wbz.documents_positions
							on document = i.id
						where status = 1
						group by extract(year from dateissue), dateissue
					) as q
					group by year", sqlConn);
                using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                sqlDA.Fill(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Pobiera sumę ilości towarów
        /// </summary>
        /// <returns></returns>
        internal static double GetStatsArticlesTotal()
        {
            var result = 0.0;

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"select coalesce(sum(quantity),0)
					from wbz.documents i
					inner join wbz.documents_positions
						on document=i.id
					where status=1", sqlConn);
                result = Convert.ToDouble(sqlCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="from">???</param>
        /// <param name="to">???</param>
        internal static DataTable GetDonationSumContractor(DateTime from, DateTime to)
        {
            var result = new DataTable();

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"SELECT DISTINCT (SELECT c.name FROM wbz.contractors c WHERE c.id = i.contractor)
					FROM wbz.documents i 
					WHERE i.archival = false and (dateissue >= '@from' and dateissue <= '@to')", sqlConn);
                sqlCmd.CommandText = sqlCmd.CommandText.Replace("@from", from.ToString("yyyy.MM.dd"));
                sqlCmd.CommandText = sqlCmd.CommandText.Replace("@to", to.ToString("yyyy.MM.dd"));
                using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                sqlDA.Fill(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="date">???</param>
        /// <param name="contractor">???</param>
        internal static DataRow GetDonationSumContractorValue(DateTime date, string contractor)
        {
            var dt = new DataTable();
            DataRow result = null;

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"SELECT 
					SUM((SELECT SUM(ip.quantity) FROM wbz.documents_positions ip WHERE i.id = ip.document)) as quantity,
					SUM((SELECT SUM(ip.cost) FROM wbz.documents_positions ip WHERE i.id = ip.document)) as cost
					FROM wbz.documents i join wbz.contractors c ON (i.contractor = c.id)
					WHERE i.archival = false and i.dateissue = '" + date.ToString("yyyy.MM.dd") + @"' and c.name = @contractor and c.archival = false", sqlConn);
                //sqlCmd.Parameters.AddWithValue("date", date);
                sqlCmd.Parameters.AddWithValue("contractor", contractor);
                using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                sqlDA.Fill(dt);
                result = dt.Rows[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="from">???</param>
        /// <param name="to">???</param>
        internal static DataTable GetDonationSumDate(DateTime from, DateTime to)
        {
            var result = new DataTable();

            try
            {
                using var sqlConn = ConnOpenedWBZ;
                var sqlCmd = new NpgsqlCommand(@"SELECT DISTINCT to_char(i.dateissue,'dd.MM.yyyy') as day, i.dateissue
					FROM wbz.documents i
					WHERE i.archival = false and (i.dateissue >= '@from' and i.dateissue <= '@to')
					ORDER BY dateissue asc", sqlConn);
                sqlCmd.CommandText = sqlCmd.CommandText.Replace("@from", from.ToString("yyyy.MM.dd"));
                sqlCmd.CommandText = sqlCmd.CommandText.Replace("@to", to.ToString("yyyy.MM.dd"));
                using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
                sqlDA.Fill(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }
        #endregion

        #region basic
        [STAThread]
        internal static void Error(string msg, Exception ex, MV module, int instanceID = 0, bool showWin = true, bool saveToSQL = true)
        {
            try
            {
                var moduleLogs = Config.GetModule(nameof(Modules.Logs));
                var error = new M_Log()
                {
                    ID = NewInstanceID(moduleLogs),
                    InstanceID = instanceID,
                    Module = module,
                    Type = 2,
                    UserID = Config.User.ID
                };

                var d = Application.Current.Dispatcher;
                d.BeginInvoke((Action)OpenErrorWindow);

                void OpenErrorWindow()
                {
#if DEBUG
                    error.Content = ex.ToString();
#else
					error.Content = ex.Message;
#endif
                    if (showWin)
                        new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"{msg}:{Environment.NewLine}{error.Content}").ShowDialog();
                    if (saveToSQL)
                        SetInstance(moduleLogs, error, Commands.Type.NEW);
                }
            }
            catch { }
        }
        #endregion

        #region modules
        /// <summary>
        /// Pobiera listę ID i wartości (zazwyczaj ID i Name)
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="column">Kolumna z której będą pobierane wartości</param>
        /// <param name="filter">Filtr SQL</param>
        internal static List<MV> ComboSource(MV module, string column, string filter, bool allowEmpty)
        {
            using var sqlConn = ConnOpenedWBZ;
            var query = $@"select id as value, {column} as display
				from wbz.{module.Value}
				where {filter ?? "true"}
				order by 2 asc";
            using var sqlDA = new NpgsqlDataAdapter(query, sqlConn);
            if (allowEmpty)
                sqlDA.SelectCommand.CommandText = "select 0 as value, '' as display union " + sqlDA.SelectCommand.CommandText;

            var dt = new DataTable();
            sqlDA.Fill(dt);
            return new List<MV>(dt.ToList<MV>());
        }

        /// <summary>
        /// Select query mode
        /// </summary>
        internal enum SelectMode { SIMPLE, EXTENDED, COUNT, COMBO }

        /// <summary>
        /// Pobiera listę instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="mode">Tryb zapytania select</param>
        /// <param name="filter">Filtr SQL</param>
        /// <param name="displayed">Liczba obecnie wyświetlonych rekordów</param>
        private static NpgsqlCommand ListCommand(MV module, SelectMode mode, M_Filter filter, int displayed)
        {
            var a = module.Alias;
            var query = "select " + module.Name switch
            {
                /// ARTICLES
                nameof(Modules.Articles) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.codename as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.codename, {a}.name, {a}.ean, coalesce(nullif(wbz.ArtDefMeaNam({a}.id),''), 'kg') as measure,
					coalesce(sum(sa.quantity), 0) as quantityraw, coalesce(sum(sa.quantity) / wbz.ArtDefMeaCon({a}.id), 0) as quantity,
					coalesce(sum(sa.reserved), 0) as reservedraw, coalesce(sum(sa.reserved) / wbz.ArtDefMeaCon({a}.id), 0) as reserved,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.codename, {a}.name, {a}.ean, coalesce(nullif(wbz.ArtDefMeaNam({a}.id),''), 'kg') as measure,
					coalesce(sum(sa.quantity), 0) as quantityraw, coalesce(sum(sa.quantity) / wbz.ArtDefMeaCon({a}.id), 0) as quantity,
					coalesce(sum(sa.reserved), 0) as reservedraw, coalesce(sum(sa.reserved) / wbz.ArtDefMeaCon({a}.id), 0) as reserved,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.articles {a}
					left join wbz.icons i on {a}.icon_id=i.id
					left join wbz.stores_articles sa on {a}.id=sa.article_id
                    --{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
					group by {a}.id, i.id
				",
                /// ATTACHMENTS
                nameof(Modules.Attachments) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.user_id, {a}.module_alias, {a}.instance_id, {a}.name,
					{a}.format, {a}.path, {a}.size, null as content
				" :
                $@"
					{a}.id, {a}.user_id, {a}.module_alias, {a}.instance_id, {a}.name,
					{a}.format, {a}.path, {a}.size, null as content
				") +
                $@"
					from wbz.attachments {a}
					left join wbz.users u on {a}.user_id = u.id	
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
				",
                /// ATTRIBUTES_CLASSES
                nameof(Modules.AttributesClasses) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.module_alias, {a}.name, {a}.type, {a}.values,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.module_alias, {a}.name, {a}.type, {a}.""values"",
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.attributes_classes {a}
					left join wbz.icons i on {a}.icon=i.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
				",
                /// CONTRACTORS
                nameof(Modules.Contractors) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.codename as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.codename, {a}.name, {a}.branch, {a}.nip, {a}.regon, {a}.postcode, {a}.city, {a}.address,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.codename, {a}.name, {a}.branch, {a}.nip, {a}.regon, {a}.postcode, {a}.city, {a}.address,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.contractors {a}
					left join wbz.icons i on {a}.icon_id=i.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
				",
                /// DISTRIBUTIONS
                nameof(Modules.Distributions) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.name, {a}.datereal, {a}.status,
					count(distinct dp.family_id) as familiescount, sum(members) as memberscount,
					count(dp.*) as positionscount, sum(dp.quantity) as weight,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.name, {a}.datereal, {a}.status,
					count(distinct dp.family_id) as familiescount, sum(members) as memberscount,
					count(dp.*) as positionscount, sum(dp.quantity) as weight,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.distributions {a}
					left join wbz.distributions_positions dp on {a}.id=dp.distribution_id
					left join wbz.icons i on {a}.icon_id=i.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
					group by {a}.id
				",
                /// DOCUMENTS
                nameof(Modules.Documents) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.name, {a}.store_id, s.name as store_name, {a}.contractor_id, c.name as contractor_name,
					{a}.type, {a}.dateissue, {a}.status, count(dp.*) as positionscount, sum(dp.quantity) as weight, sum(dp.net) as net,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.name, {a}.store_id, s.name as store_name, {a}.contractor_id, c.name as contractor_name,
					{a}.type, {a}.dateissue, {a}.status, count(dp.*) as positionscount, sum(dp.quantity) as weight, sum(dp.net) as net,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.documents {a}
					left join wbz.documents_positions dp on {a}.id=dp.document_id
					left join wbz.contractors c on {a}.contractor_id=c.id
					left join wbz.icons i on {a}.icon_id=i.id
					left join wbz.stores s on {a}.store_id=s.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
					group by {a}.id, c.id, s.id
				",
                /// EMPLOYEES
                nameof(Modules.Employees) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, concat({a}.lastname, ' ', {a}.forename) as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.user_id, concat(u.lastname, ' ', u.forename) as username,
					{a}.forename, {a}.lastname, {a}.department, {a}.position,
					{a}.email, {a}.phone, {a}.postcode, {a}.city, {a}.address,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.user_id, concat(u.lastname, ' ', u.forename) as username,
					{a}.forename, {a}.lastname, {a}.department, {a}.position,
					{a}.email, {a}.phone, {a}.postcode, {a}.city, {a}.address,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.employees {a}
					left join wbz.icons i on {a}.icon_id=i.id
					left join wbz.users u on {a}.user_id=u.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
				",
                /// FAMILIES
                nameof(Modules.Families) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.lastname as display"
                : mode == SelectMode.SIMPLE ?
                $@"
					{a}.id, {a}.declarant, {a}.lastname, {a}.members, {a}.postcode, {a}.city, {a}.address,
					{a}.status, {a}.c_sms, {a}.c_call, {a}.c_email, max(d.datereal) as donationlast, sum(dp.quantity) as donationweight,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				" :
                $@"
					{a}.id, {a}.declarant, {a}.lastname, {a}.members, {a}.postcode, {a}.city, {a}.address,
					{a}.status, {a}.c_sms, {a}.c_call, {a}.c_email, max(d.datereal) as donationlast, sum(dp.quantity) as donationweight,
					{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
				") +
                $@"
					from wbz.families {a}
					left join wbz.icons i on {a}.icon_id=i.id
					left join wbz.distributions_positions dp on {a}.id=dp.family_id
					left join wbz.distributions d on dp.distribution_id=d.id
					--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
					group by {a}.id
				",
                /// GROUPS
                nameof(Modules._submodules.Groups) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.module_alias, {a}.name, {a}.instance_id, {a}.owner_id,
        			case when trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\') = '' then ''
        				else concat(trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\'), '\') end as path,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		" :
                $@"
        			{a}.id, {a}.module_alias, {a}.name, {a}.instance_id, {a}.owner_id,
        			case when trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\') = '' then ''
        				else concat(trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\'), '\') end as path,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		") +
                $@"
        			from wbz.groups {a}
        			left join wbz.icons i on {a}.icon_id=i.id
        			left join wbz.groups g4 on g4.id={a}.owner_id
        			left join wbz.groups g3 on g3.id=g4.owner_id
        			left join wbz.groups g2 on g2.id=g3.owner_id
        			left join wbz.groups g1 on g1.id=g2.owner_id
        			where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
        		",
                /// ICONS
                nameof(Modules.Icons) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.name as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.module_alias, {a}.name, {a}.format, {a}.path,
        			{a}.content, {a}.height, {a}.width, {a}.size,
        			{a}.is_archival, {a}.comment
        		" :
                $@"
        			{a}.id, {a}.module_alias, {a}.name, {a}.format, {a}.path,
        			{a}.content, {a}.height, {a}.width, {a}.size,
        			{a}.is_archival, {a}.comment
        		") +
                $@"
        			from wbz.icons {a}
        			--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
        		",
                /// LOGS
                nameof(Modules.Logs) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.datecreated as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.user_id, concat(u.lastname, ' ', u.forename) as user_name, {a}.module_alias, {a}.instance_id,
                    {a}.type, {a}.content, {a}.datecreated
        		" :
                $@"
        			{a}.id, {a}.user_id, concat(u.lastname, ' ', u.forename) as user_name, {a}.module_alias, {a}.instance_id,
                    {a}.type, {a}.content, {a}.datecreated
        		") +
                $@"
        			from wbz.logs {a}
        			left join wbz.users u on {a}.user_id = u.id
        			where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
        		",
                /// STORES
                nameof(Modules.Stores) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.codename as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.codename, {a}.name, {a}.postcode, {a}.city, {a}.address,
        			coalesce(sum(sa.quantity),0) as quantity, coalesce(sum(sa.reserved),0) as reserved,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		" :
                $@"
        			{a}.id, {a}.codename, {a}.name, {a}.postcode, {a}.city, {a}.address,
        			coalesce(sum(sa.quantity),0) as quantity, coalesce(sum(sa.reserved),0) as reserved,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		") +
                $@"
        			from wbz.stores {a}
        			left join wbz.icons i on {a}.icon_id=i.id
        			left join wbz.stores_articles sa on {a}.id = sa.store_id
        			--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
        			group by {a}.id, i.id
        		",
                /// USERS
                nameof(Modules.Users) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, concat({a}.lastname, ' ', {a}.forename) as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.login, '' as newpass, {a}.forename, {a}.lastname,
        			{a}.email, {a}.phone, {a}.is_archival, {a}.is_blocked
        		" :
                $@"
        			{a}.id, {a}.login, '' as newpass, {a}.forename, {a}.lastname,
        			{a}.email, {a}.phone, {a}.is_archival, {a}.is_blocked
        		") +
                $@"
        			from wbz.users {a}
        			--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
        		",
                /// VEHICLES
                nameof(Modules.Vehicles) =>
                (mode == SelectMode.COUNT ? "count (*) "
                : mode == SelectMode.COMBO ? $"{a}.id as value, {a}.register as display"
                : mode == SelectMode.SIMPLE ?
                $@"
        			{a}.id, {a}.register, {a}.brand, {a}.model, {a}.capacity,
        			{a}.forwarder_id, c.codename as forwarder_name,
        			{a}.driver_id, e.lastname || ' ' || e.forename as driver_name,
        			{a}.prodyear,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		" :
                $@"
        			{a}.id, {a}.register, {a}.brand, {a}.model, {a}.capacity,
        			c.id as forwarderid, c.codename as forwardername,
        			e.id as driverid, e.lastname || ' ' || e.forename as drivername,
        			{a}.prodyear,
        			{a}.is_archival, {a}.comment, {a}.icon_id, i.content as icon_content
        		") +
                $@"
        			from wbz.vehicles {a}
        			left join wbz.contractors c on {a}.forwarder_id=c.id
        			left join wbz.employees e on {a}.driver_id=e.id
        			left join wbz.icons i on {a}.icon_id=i.id
        			--{(filter.ShowGroup > 0 ? $"inner join wbz.groups g on g.module_alias='{a}' and g.owner_id={filter.ShowGroup} and g.instance_id={a}.id" : string.Empty)}
					where {filter.AutoFilterString ?? "true"} and {filter.Content ?? "true"}
                        --and {(filter.ShowArchival || mode == SelectMode.EXTENDED ? "true" : $"{a}.is_archival=false")}
        		",
                _ => throw new NotImplementedException(),
            };

            /// order by & limit & offset
            if (mode != SelectMode.COUNT)
            {
                //query += $" order by {string.Join(',', filter.Sorting.Cast<string>())}";
                if (filter.Limit > 0)
                    query += $" limit {filter.Limit} offset {displayed}";
            }
            /// add params
            var sqlCmd = new NpgsqlCommand(query);
            if (filter.AutoFilterParams != null)
                foreach (var param in filter.AutoFilterParams)
                    sqlCmd.Parameters.AddWithValue(param.Name.ToString(), param.Value);

            return sqlCmd;
        }

        /// <summary>
        /// Pobiera listę instancji w formie Combo
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="filter">Filtr SQL</param>
        internal static List<MV> ComboInstances<T>(MV module, M_Filter filter) where T : class, new()
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlDA = new NpgsqlDataAdapter(ListCommand(module, SelectMode.COMBO, filter, 0));
            sqlDA.SelectCommand.Connection = sqlConn;

            var dt = new DataTable();
            sqlDA.Fill(dt);

            return new List<MV>(dt.ToList<MV>("_"));
        }
        internal static List<MV> ComboInstances<T>(MV module, string filter) where T : class, new() => ComboInstances<T>(module, new M_Filter(module) { AutoFilterString = filter });

        /// <summary>
        /// Liczy ilość instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="filter">Filtr SQL</param>
        internal static int CountInstances(MV module, M_Filter filter)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlCmd = ListCommand(module, SelectMode.COUNT, filter, 0);
            sqlCmd.Connection = sqlConn;

            return Convert.ToInt32(sqlCmd.ExecuteScalar());
        }
        internal static int CountInstances(MV module, string filter) => CountInstances(module, new M_Filter(module) { AutoFilterString = filter });

        /// <summary>
        /// Pobiera listę instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="filter">Filtr SQL</param>
        /// <param name="displayed">Liczba obecnie wyświetlonych rekordów</param>
        internal static List<T> ListInstances<T>(MV module, M_Filter filter, int displayed = 0) where T : class, new()
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlDA = new NpgsqlDataAdapter(ListCommand(module, SelectMode.SIMPLE, filter, displayed));
            sqlDA.SelectCommand.Connection = sqlConn;

            var dt = new DataTable();
            sqlDA.Fill(dt);

            return new List<T>(dt.ToList<T>("_"));
        }
        internal static List<T> ListInstances<T>(MV module, string filter, int displayed = 0) where T : class, new() => ListInstances<T>(module, new M_Filter(module) { AutoFilterString = filter }, displayed);

        /// <summary>
        /// Pobiera dane o instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        internal static T GetInstance<T>(MV module, int instanceID) where T : class, new()
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlDA = new NpgsqlDataAdapter(ListCommand(module, SelectMode.EXTENDED, new M_Filter(module) { AutoFilterString = $"{module.Alias}.id={instanceID}" }, 0));
            sqlDA.SelectCommand.Connection = sqlConn;

            var dt = new DataTable();
            sqlDA.Fill(dt);

            return new List<T>(dt.ToList<T>("_"))?[0];
        }

        /// <summary>
        /// Pobiera listę pozycji instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        internal static DataTable GetInstancePositions(MV module, int instanceID)
        {
            var result = new DataTable();

            using var sqlConn = ConnOpenedWBZ;
            string query = module.Value switch
            {
                nameof(Modules.Articles) => @"
					select am.id, am.name, am.converter, am.is_default,
						sa.quantity / coalesce(nullif(am.converter, 0), 1) as quantity, sa.reserved / coalesce(nullif(am.converter, 0), 1) as reserved
					from wbz.articles a
					inner join wbz.articles_measures am on a.id = am.article_id
					left join wbz.stores_articles sa on a.id = sa.article_id
					where a.id=@id",
                nameof(Modules.AttributesClasses) => @"
					select id, value, is_archival
					from wbz.attributes_values av
					where class=@id",
                nameof(Modules.Distributions) => @"
					select id, pos, family_id, (select lastname from wbz.families where id=dp.family_id) as family_name, members,
						store_id, (select name from wbz.stores where id=dp.store_id) as store_name,
						article_id, (select name from wbz.articles where id=dp.article_id) as article_name,
						quantity / wbz.ArtDefMeaCon(dp.article_id) as quantity, coalesce(nullif(wbz.ArtDefMeaNam(dp.article_id),''), 'kg') as measure, status
					from wbz.distributions_positions dp
					where distribution=@id",
                nameof(Modules.Documents) => @"
					select id, pos, article_id, (select name from wbz.articles where id=dp.article_id) as article_name,
						quantity / wbz.ArtDefMeaCon(dp.article_id) as quantity, coalesce(nullif(wbz.ArtDefMeaNam(dp.article_id),''), 'kg') as measure, net
					from wbz.documents_positions dp
					where document=@id",
                _ => throw new NotImplementedException(),
            };
            using (var sqlDA = new NpgsqlDataAdapter(query, sqlConn))
            {
                sqlDA.SelectCommand.Parameters.AddWithValue("id", instanceID);
                sqlDA.Fill(result);
            }

            /// ARTICLES
            if (module.Name == nameof(Modules.Articles))
            {
                result.Columns["converter"].DefaultValue = 1.0;
                result.Columns["is_default"].DefaultValue = false;
            }
            else if (module.Name == nameof(Modules.Distributions))
            {

            }

            return result;
        }

        /// <summary>
        /// Pobiera z sekwencji ID nowej instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        internal static int NewInstanceID(MV module)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlCmd = new NpgsqlCommand($"select nextval('wbz.{module.Value}_id_seq')", sqlConn);
            return Convert.ToInt32(sqlCmd.ExecuteScalar());
        }

        /// <summary>
        /// Ustawia dane instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instance">Instancja</param>
        /// <param name="mode">Tryb</param>
        internal static bool SetInstance<T>(MV module, T instance, Commands.Type mode)
        {
            string query;
            int oldstatus = 0;

            using (var sqlConn = ConnOpenedWBZ)
            using (var sqlTran = sqlConn.BeginTransaction())
            {
                NpgsqlCommand sqlCmd;

                switch (module.Name)
                {
                    /// ARTICLES
                    case nameof(Modules.Articles):
                        var article = instance as M_Article;
                        query = @"insert into wbz.articles (id, codename, name, ean, is_archival, comment, icon_id)
								values (@id, @codename, @name, @ean, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set codename=@codename, name=@name, ean=@ean,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", article.ID);
                            sqlCmd.Parameters.AddWithValue("codename", article.Codename);
                            sqlCmd.Parameters.AddWithValue("name", article.Name);
                            sqlCmd.Parameters.AddWithValue("ean", article.EAN);
                            sqlCmd.Parameters.AddWithValue("is_archival", article.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", article.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", article.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, article.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} towar: {article.Name}.", sqlConn, sqlTran);

                        ///measures
                        foreach (DataRow measure in article.Measures.Rows)
                        {
                            ///add
                            if (measure.RowState == DataRowState.Added)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"insert into wbz.articles_measures (article_id, name, converter, is_default)
										values (@article_id, @name, @converter, @is_default)", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("article_id", article.ID);
                                    sqlCmd.Parameters.AddWithValue("name", measure["name"]);
                                    sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
                                    sqlCmd.Parameters.AddWithValue("is_default", measure["is_default"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, article.ID, $"Dodano jednostkę miary {measure["name"]}.", sqlConn, sqlTran);
                            }
                            ///edit
                            else if (measure.RowState == DataRowState.Modified)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"update wbz.articles_measures
										set name=@name, converter=@converter, is_default=@is_default
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("id", measure["id", DataRowVersion.Original]);
                                    sqlCmd.Parameters.AddWithValue("name", measure["name"]);
                                    sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
                                    sqlCmd.Parameters.AddWithValue("is_default", measure["is_default"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, article.ID, $"Edytowano jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }
                            ///delete
                            else if (measure.RowState == DataRowState.Deleted)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"delete from wbz.articles_measures
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("id", measure["id", DataRowVersion.Original]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, article.ID, $"Usunięto jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }
                        }
                        break;
                    /// ATTACHMENTS
                    case nameof(Modules.Attachments):
                        query = @"";
                        break;
                    /// ATTRIBUTES_CLASSES
                    case nameof(Modules.AttributesClasses):
                        var attributeClass = instance as M_AttributeClass;
                        query = @"insert into wbz.attributes_classes (id, module_alias, name, type, is_required, is_archival, comment, icon_id)
								values (@id, @module_alias, @name, @type, @is_required, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set module_alias=@module_alias, name=@name, type=@type, is_required=@is_required,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", attributeClass.ID);
                            sqlCmd.Parameters.AddWithValue("module_alias", attributeClass.Module.Alias);
                            sqlCmd.Parameters.AddWithValue("name", attributeClass.Name);
                            sqlCmd.Parameters.AddWithValue("type", attributeClass.Type);
                            sqlCmd.Parameters.AddWithValue("is_required", attributeClass.IsRequired);
                            sqlCmd.Parameters.AddWithValue("is_archival", attributeClass.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", attributeClass.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", attributeClass.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, attributeClass.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} klasę atrybutu: {attributeClass.Name}.", sqlConn, sqlTran);

                        ///values
                        foreach (DataRow value in attributeClass.Values.Rows)
                        {
                            ///add
                            if (value.RowState == DataRowState.Added)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"insert into wbz.attributes_values (class_id, value, is_archival)
										values (@class_id, @value, @is_archival)", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("class_id", attributeClass.ID);
                                    sqlCmd.Parameters.AddWithValue("value", value["name"]);
                                    sqlCmd.Parameters.AddWithValue("is_archival", value["is_archival"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, attributeClass.ID, $"Dodano wartość {value["name"]}.", sqlConn, sqlTran);
                            }
                            ///edit
                            else if (value.RowState == DataRowState.Modified)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"update wbz.attributes_values
										set value=@value, is_archival=@is_archival
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("id", value["id", DataRowVersion.Original]);
                                    sqlCmd.Parameters.AddWithValue("value", value["name"]);
                                    sqlCmd.Parameters.AddWithValue("is_archival", value["is_archival"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, attributeClass.ID, $"Edytowano wartość {value["name", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }
                            ///delete
                            else if (value.RowState == DataRowState.Deleted)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes_values
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.AddWithValue("id", value["id", DataRowVersion.Original]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, attributeClass.ID, $"Usunięto wartość {value["name", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }
                        }
                        break;
                    /// CONTRACTORS
                    case nameof(Modules.Contractors):
                        var contractor = instance as M_Contractor;
                        query = @"insert into wbz.contractors (id, codename, name, branch, nip, regon, postcode, city, address, is_archival, comment, icon_id)
								values (@id, @codename, @name, @branch, @nip, @regon, @postcode, @city, @address, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set codename=@codename, name=@name, branch=@branch, nip=@nip, regon=@regon,
									postcode=@postcode, city=@city, address=@address,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", contractor.ID);
                            sqlCmd.Parameters.AddWithValue("codename", contractor.Codename);
                            sqlCmd.Parameters.AddWithValue("name", contractor.Name);
                            sqlCmd.Parameters.AddWithValue("branch", contractor.Branch);
                            sqlCmd.Parameters.AddWithValue("nip", contractor.NIP);
                            sqlCmd.Parameters.AddWithValue("regon", contractor.REGON);
                            sqlCmd.Parameters.AddWithValue("postcode", contractor.Postcode);
                            sqlCmd.Parameters.AddWithValue("city", contractor.City);
                            sqlCmd.Parameters.AddWithValue("address", contractor.Address);
                            sqlCmd.Parameters.AddWithValue("is_archival", contractor.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", contractor.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", contractor.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, contractor.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} kontrahenta: {contractor.Name}.", sqlConn, sqlTran);
                        break;
                    /// DISTRIBUTIONS
                    case nameof(Modules.Distributions):
                        var distribution = instance as M_Distribution;
                        using (sqlCmd = new NpgsqlCommand(@"select status from wbz.distributions where id=@id", sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", distribution.ID);
                            oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
                        }
                        query = @"insert into wbz.distributions (id, name, datereal, status, is_archival, comment, icon_id)
								values (@id, @name, @datereal, @status, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set name=@name, datereal=@datereal, status=@status, is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", distribution.ID);
                            sqlCmd.Parameters.AddWithValue("name", distribution.Name);
                            sqlCmd.Parameters.AddWithValue("datereal", distribution.DateReal);
                            sqlCmd.Parameters.AddWithValue("status", distribution.Status);
                            sqlCmd.Parameters.AddWithValue("is_archival", distribution.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", distribution.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", distribution.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, distribution.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} dystrybucję: {distribution.Name}.", sqlConn, sqlTran);

                        ///positions
                        foreach (var posfam in distribution.Families)
                            foreach (DataRow position in posfam.Positions.Rows)
                            {
                                ///add
                                if (position.RowState == DataRowState.Added)
                                {
                                    sqlCmd = new NpgsqlCommand(@"insert into wbz.distributions_positions (distribution_id, pos, family_id, members, store_id, article_id, quantity, status)
											values (@distribution_id, @pos, @family_id, @members, @store_id, @article_id, (@quantity * wbz.ArtDefMeaCon(@article_id)), @status)", sqlConn, sqlTran);
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("distribution_id", distribution.ID);
                                    sqlCmd.Parameters.AddWithValue("pos", position["pos"]);
                                    sqlCmd.Parameters.AddWithValue("family_id", posfam.FamilyID);
                                    sqlCmd.Parameters.AddWithValue("members", posfam.Members);
                                    sqlCmd.Parameters.AddWithValue("store_id", position["store_id"]);
                                    sqlCmd.Parameters.AddWithValue("article_id", position["article_id"]);
                                    sqlCmd.Parameters.AddWithValue("quantity", position["quantity"]);
                                    sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
                                    sqlCmd.Parameters.AddWithValue("status", posfam.Status);
                                    sqlCmd.ExecuteNonQuery();
                                    SetLog(Config.User.ID, module, distribution.ID, $"Dodano pozycję {position["position"]}.", sqlConn, sqlTran);
                                }
                                ///edit
                                else if (position.RowState == DataRowState.Modified)
                                {
                                    sqlCmd = new NpgsqlCommand(@"update wbz.distributions_positions
											set pos=@pos, family_id=@family_id, members=@members, store_id=@store_id, article_id=@article_id, quantity=(@quantity * wbz.ArtDefMeaCon(@article)), status=@status
											where id=@id", sqlConn, sqlTran);
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
                                    sqlCmd.Parameters.AddWithValue("pos", position["pos"]);
                                    sqlCmd.Parameters.AddWithValue("family_id", posfam.FamilyID);
                                    sqlCmd.Parameters.AddWithValue("members", posfam.Members);
                                    sqlCmd.Parameters.AddWithValue("store_id", position["store"]);
                                    sqlCmd.Parameters.AddWithValue("article_id", position["article"]);
                                    sqlCmd.Parameters.AddWithValue("quantity", position["quantity"]);
                                    sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
                                    sqlCmd.Parameters.AddWithValue("status", posfam.Status);
                                    sqlCmd.ExecuteNonQuery();
                                    SetLog(Config.User.ID, module, distribution.ID, $"Edytowano pozycję {position["position", DataRowVersion.Original]}.", sqlConn, sqlTran);
                                }
                                ///delete
                                else if (position.RowState == DataRowState.Deleted)
                                {
                                    sqlCmd = new NpgsqlCommand(@"delete from wbz.distributions_positions
											where id=@id", sqlConn, sqlTran);
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
                                    sqlCmd.ExecuteNonQuery();
                                    SetLog(Config.User.ID, module, distribution.ID, $"Usunięto pozycję {position["position", DataRowVersion.Original]}.", sqlConn, sqlTran);
                                }

                                ///update articles quantity
                                if (oldstatus != distribution.Status)
                                {
                                    if (oldstatus <= 0 && distribution.Status > 0)
                                        ChangeArticleQuantity((int)position["store_id"], (int)position["article_id"], -Convert.ToDouble(position["quantity"]), (string)position["measure"], false, sqlConn, sqlTran);
                                    else if (oldstatus > 0 && distribution.Status < 0)
                                        ChangeArticleQuantity((int)position["store_id"], (int)position["article_id"], Convert.ToDouble(position["quantity"]), (string)position["measure"], false, sqlConn, sqlTran);
                                }
                            }
                        break;
                    /// DOCUMENTS
                    case nameof(Modules.Documents):
                        var document = instance as M_Document;
                        using (sqlCmd = new NpgsqlCommand(@"select status from wbz.documents where id=@id", sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", document.ID);
                            oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
                        }
                        query = @"insert into wbz.documents (id, name, type, store_id, contractor_id, dateissue, status, is_archival, comment, icon_id)
								values (@id, @name, @type, @store_id, @contractor_id, @dateissue, @status, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set name=@name, type=@type, store_id=@store_id, contractor_id=@contractor_id, dateissue=@dateissue, status=@status,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", document.ID);
                            sqlCmd.Parameters.AddWithValue("name", document.Name);
                            sqlCmd.Parameters.AddWithValue("type", document.Type);
                            sqlCmd.Parameters.AddWithValue("store_id", document.StoreID);
                            sqlCmd.Parameters.AddWithValue("contractor_id", document.ContractorID);
                            sqlCmd.Parameters.AddWithValue("dateissue", document.DateIssue);
                            sqlCmd.Parameters.AddWithValue("status", document.Status);
                            sqlCmd.Parameters.AddWithValue("is_archival", document.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", document.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", document.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, document.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} dokument: {document.Name}.", sqlConn, sqlTran);

                        ///positions
                        foreach (DataRow position in document.Positions.Rows)
                        {
                            ///add
                            if (position.RowState == DataRowState.Added)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"insert into wbz.documents_positions (document_id, pos, article_id, quantity, net)
										values (@document_id, @pos, @article_id, (@quantity * wbz.ArtDefMeaCon(@article_id)),
											@net)", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("document_id", document.ID);
                                    sqlCmd.Parameters.AddWithValue("pos", position["pos"]);
                                    sqlCmd.Parameters.AddWithValue("article_id", position["article_id"]);
                                    sqlCmd.Parameters.AddWithValue("quantity", position["quantity"]);
                                    sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
                                    sqlCmd.Parameters.AddWithValue("net", position["net"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, document.ID, $"Dodano pozycję {position["pos"]}.", sqlConn, sqlTran);
                            }
                            ///edit
                            else if (position.RowState == DataRowState.Modified)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"update wbz.documents_positions
										set pos=@pos, article_id=@article_id, quantity=(@quantity * wbz.ArtDefMeaCon(@article_id)),
											net=@net
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
                                    sqlCmd.Parameters.AddWithValue("pos", position["pos"]);
                                    sqlCmd.Parameters.AddWithValue("article_id", position["article_id"]);
                                    sqlCmd.Parameters.AddWithValue("quantity", position["quantity"]);
                                    sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
                                    sqlCmd.Parameters.AddWithValue("net", position["net"]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, document.ID, $"Edytowano pozycję {position["position", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }
                            ///delete
                            else if (position.RowState == DataRowState.Deleted)
                            {
                                using (sqlCmd = new NpgsqlCommand(@"delete from wbz.documents_positions
										where id=@id", sqlConn, sqlTran))
                                {
                                    sqlCmd.Parameters.Clear();
                                    sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
                                    sqlCmd.ExecuteNonQuery();
                                }
                                SetLog(Config.User.ID, module, document.ID, $"Usunięto pozycję {position["pos", DataRowVersion.Original]}.", sqlConn, sqlTran);
                            }

                            ///update articles quantity
                            if (oldstatus != document.Status)
                            {
                                if (oldstatus <= 0 && document.Status > 0)
                                    ChangeArticleQuantity(document.StoreID, (int)position["article_id"], Convert.ToDouble(position["quantity"]), (string)position["measure"], false, sqlConn, sqlTran);
                                else if (oldstatus > 0 && document.Status < 0)
                                    ChangeArticleQuantity(document.StoreID, (int)position["article_id"], -Convert.ToDouble(position["quantity"]), (string)position["measure"], false, sqlConn, sqlTran);
                            }
                        }
                        break;
                    /// EMPLOYEES
                    case nameof(Modules.Employees):
                        var employee = instance as M_Employee;
                        query = @"insert into wbz.employees (id, forename, lastname, department, position,
									email, phone, city, address, postcode, is_archival, comment, icon_id)
								values (@id, @forename, @lastname, @department, @position,
									@email, @phone, @city, @address, @postcode, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set forename=@forename, lastname=@lastname, department=@department, position=@position,
									email=@email, phone=@phone, city=@city, address=@address, postcode=@postcode,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", employee.ID);
                            sqlCmd.Parameters.AddWithValue("forename", employee.Forename);
                            sqlCmd.Parameters.AddWithValue("lastname", employee.Lastname);
                            sqlCmd.Parameters.AddWithValue("department", employee.Department);
                            sqlCmd.Parameters.AddWithValue("position", employee.Position);
                            sqlCmd.Parameters.AddWithValue("email", employee.Email);
                            sqlCmd.Parameters.AddWithValue("phone", employee.Phone);
                            sqlCmd.Parameters.AddWithValue("city", employee.City);
                            sqlCmd.Parameters.AddWithValue("address", employee.Address);
                            sqlCmd.Parameters.AddWithValue("postcode", employee.Postcode);
                            sqlCmd.Parameters.AddWithValue("is_archival", employee.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", employee.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", employee.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, employee.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} pracownika: {employee.Name}.", sqlConn, sqlTran);
                        break;
                    /// FAMILIES
                    case nameof(Modules.Families):
                        var family = instance as M_Family;
                        query = @"insert into wbz.families (id, declarant, lastname, members, postcode, city, address,
									status, c_sms, c_call, c_email, is_archival, comment, icon_id)
								values (@id, @declarant, @lastname, @members, @postcode, @city, @address,
									@status, @c_sms, @c_call, @c_email, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set declarant=@declarant, lastname=@lastname, members=@members, postcode=@postcode, city=@city, address=@address,
									status=@status, c_sms=@c_sms, c_call=@c_call, c_email=@c_email,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", family.ID);
                            sqlCmd.Parameters.AddWithValue("declarant", family.Declarant);
                            sqlCmd.Parameters.AddWithValue("lastname", family.Lastname);
                            sqlCmd.Parameters.AddWithValue("members", family.Members);
                            sqlCmd.Parameters.AddWithValue("postcode", family.Postcode);
                            sqlCmd.Parameters.AddWithValue("city", family.City);
                            sqlCmd.Parameters.AddWithValue("address", family.Address);
                            sqlCmd.Parameters.AddWithValue("status", family.Status);
                            sqlCmd.Parameters.AddWithValue("c_sms", family.C_SMS);
                            sqlCmd.Parameters.AddWithValue("c_call", family.C_Call);
                            sqlCmd.Parameters.AddWithValue("c_email", family.C_Email);
                            sqlCmd.Parameters.AddWithValue("is_archival", family.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", family.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", family.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, family.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} rodzinę: {family.Lastname}.", sqlConn, sqlTran);
                        break;
                    /// GROUPS
                    case nameof(Modules._submodules.Groups):
                        var group = instance as M_Group;
                        query = @"insert into wbz.groups (id, module_alias, name, instance_id, owner_id, is_archival, comment, icon_id)
								values (@id, @module_alias, @name, @instance_id, @owner_id, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set module_alias=@module_alias, name=@name, instance_id=@instance_id, owner_id=@owner_id, is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", group.ID);
                            sqlCmd.Parameters.AddWithValue("module_alias", group.Module?.Alias ?? string.Empty);
                            sqlCmd.Parameters.AddWithValue("name", group.Name ?? string.Empty);
                            sqlCmd.Parameters.AddWithValue("instance_id", group.InstanceID);
                            sqlCmd.Parameters.AddWithValue("owner_id", group.OwnerID);
                            sqlCmd.Parameters.AddWithValue("is_archival", group.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", group.Comment ?? string.Empty);
                            sqlCmd.Parameters.AddWithValue("icon_id", group.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, group.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} grupę: {group.Name}.", sqlConn, sqlTran);
                        break;
                    /// ICONS
                    case nameof(Modules.Icons):
                        var icon = instance as M_Icon;
                        query = @"insert into wbz.icons (id, module_alias, name, format, path,
									content, height, width, size,
									is_archival, comment)
								values (@id, @module_alias, @name, @format, @path,
									@content, @height, @width, @size,
									@is_archival, @comment)
								on conflict(id) do
								update set module_alias=@module_alias, name=@name, format=@format, path=@path,
									content=@content, height=@height, width=@width, size=@size,
									is_archival=@is_archival, comment=@comment";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", icon.ID);
                            sqlCmd.Parameters.AddWithValue("module_alias", icon.Module.Alias);
                            sqlCmd.Parameters.AddWithValue("name", icon.Name);
                            sqlCmd.Parameters.AddWithValue("format", icon.Format);
                            sqlCmd.Parameters.AddWithValue("path", icon.Path);
                            sqlCmd.Parameters.AddWithValue("content", icon.Content);
                            sqlCmd.Parameters.AddWithValue("height", icon.Height);
                            sqlCmd.Parameters.AddWithValue("width", icon.Width);
                            sqlCmd.Parameters.AddWithValue("size", icon.Size);
                            sqlCmd.Parameters.AddWithValue("is_archival", icon.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", icon.Comment);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, icon.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} ikonę: {icon.Name}.", sqlConn, sqlTran);
                        break;
                    /// LOGS
                    case nameof(Modules.Logs):
                        var log = instance as M_Log;
                        query = @"insert into wbz.logs (user_id, module_alias, instance_id, type, content)
								values (@user_id, @module_alias, @instance_id, @type, @content)
								on conflict(id) do
								update set user_id=@user_id, module_alias=@module_alias, instance_id=@instance_id, type=@type, content=@content";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("user_id", log.UserID);
                            sqlCmd.Parameters.AddWithValue("module_alias", log.Module);
                            sqlCmd.Parameters.AddWithValue("instance_id", log.InstanceID);
                            sqlCmd.Parameters.AddWithValue("type", log.Type);
                            sqlCmd.Parameters.AddWithValue("content", log.Content);
                            sqlCmd.ExecuteNonQuery();
                        }
                        break;
                    /// STORES
                    case nameof(Modules.Stores):
                        var store = instance as M_Store;
                        query = @"insert into wbz.stores (id, codename, name, city, address, postcode, is_archival, comment, icon_id)
								values (@id, @codename, @name, @city, @address, @postcode, @is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set codename=@codename, name=@name, city=@city, address=@address, postcode=@postcode,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", store.ID);
                            sqlCmd.Parameters.AddWithValue("codename", store.Codename);
                            sqlCmd.Parameters.AddWithValue("name", store.Name);
                            sqlCmd.Parameters.AddWithValue("city", store.City);
                            sqlCmd.Parameters.AddWithValue("address", store.Address);
                            sqlCmd.Parameters.AddWithValue("postcode", store.Postcode);
                            sqlCmd.Parameters.AddWithValue("is_archival", store.IsArchival);
                            sqlCmd.Parameters.AddWithValue("comment", store.Comment);
                            sqlCmd.Parameters.AddWithValue("icon_id", store.IconID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, store.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} magazyn: {store.Name}.", sqlConn, sqlTran);
                        break;
                    /// USERS
                    case nameof(Modules.Users):
                        var user = instance as M_User;
                        query = @"insert into wbz.users (id, login, password, forename, lastname, email, phone, is_archival, is_blocked)
								values (@id, @login, @password, @forename, @lastname, @email, @phone, @is_archival, @is_blocked)
								on conflict(id) do
								update set login=@login, " + (!string.IsNullOrEmpty(user.Newpass) ? "password = @password," : "") + @"
									forename=@forename, lastname=@lastname, email=@email, phone=@phone, is_archival=@is_archival, is_blocked=@is_blocked";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", user.ID);
                            sqlCmd.Parameters.AddWithValue("login", user.Login);
                            sqlCmd.Parameters.AddWithValue("password", Global.sha256(user.Newpass));
                            sqlCmd.Parameters.AddWithValue("forename", user.Forename);
                            sqlCmd.Parameters.AddWithValue("lastname", user.Lastname);
                            sqlCmd.Parameters.AddWithValue("email", user.Email);
                            sqlCmd.Parameters.AddWithValue("phone", user.Phone);
                            sqlCmd.Parameters.AddWithValue("is_archival", user.IsArchival);
                            sqlCmd.Parameters.AddWithValue("is_blocked", user.IsBlocked);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, user.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} użytkownika: {user.Name}.", sqlConn, sqlTran);

                        /// permissions
                        using (sqlCmd = new NpgsqlCommand(@"delete from wbz.users_permissions where user_id=@user_id", sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("user_id", user.ID);
                            sqlCmd.ExecuteNonQuery();
                        }
                        foreach (string perm in user.Perms)
                        {
                            using (sqlCmd = new NpgsqlCommand(@"insert into wbz.users_permissions (user_id, perm)
									values (@user_id, @perm)", sqlConn, sqlTran))
                            {
                                sqlCmd.Parameters.AddWithValue("user_id", user.ID);
                                sqlCmd.Parameters.AddWithValue("perm", perm);
                                sqlCmd.ExecuteNonQuery();
                            }
                        }
                        break;
                    /// VEHICLES
                    case nameof(Modules.Vehicles):
                        var vehicle = instance as M_Vehicle;
                        query = @"insert into wbz.vehicles (id, register, brand, model, capacity,
									forwarder_id, driver_id, prodyear,
									is_archival, comment, icon_id)
								values (@id, @register, @brand, @model, @capacity,
									nullif(@forwarder_id, 0), nullif(@driver_id, 0), @prodyear,
									@is_archival, @comment, nullif(@icon_id, 0))
								on conflict(id) do
								update set register=@register, brand=@brand, model=@model, capacity=@capacity,
									forwarder_id=nullif(@forwarder_id, 0), driver_id=nullif(@driver_id, 0), prodyear=@prodyear,
									is_archival=@is_archival, comment=@comment, icon_id=nullif(@icon_id, 0)";
                        using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
                        {
                            sqlCmd.Parameters.AddWithValue("id", vehicle.ID);
                            sqlCmd.Parameters.AddWithValue("register", (object)vehicle.Register ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("brand", (object)vehicle.Brand ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("model", (object)vehicle.Model ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("capacity", (object)vehicle.Capacity ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("forwarder_id", (object)vehicle.ForwarderID ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("driver_id", (object)vehicle.DriverID ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("prodyear", (object)vehicle.ProdYear ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("is_archival", (object)vehicle.IsArchival ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("comment", (object)vehicle.Comment ?? DBNull.Value);
                            sqlCmd.Parameters.AddWithValue("icon_id", (object)vehicle.IconID ?? DBNull.Value);
                            sqlCmd.ExecuteNonQuery();
                        }
                        SetLog(Config.User.ID, module, vehicle.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} pojazd: {vehicle.Name}.", sqlConn, sqlTran);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                sqlTran.Commit();
            }

            return true;
        }

        /// <summary>
        /// Usunięcie instancji
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="name">Nazwa instancji dla logu</param>
        internal static bool DeleteInstance(MV module, int instanceID, string name)
        {
            bool result = false;
            string query;
            int oldstatus = 0;

            using var sqlConn = ConnOpenedWBZ;
            using var sqlTran = sqlConn.BeginTransaction();
            switch (module.Name)
            {
                /// ARTICLES
                case nameof(Modules.Articles):
                    query = @"delete from wbz.stores_articles where article_id=@id;
						delete from wbz.articles where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto towar: {name}", sqlConn, sqlTran);
                    break;
                /// ATTACHMENTS
                case nameof(Modules.Attachments):
                    query = @"delete from wbz.attachments where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto załącznik: {name}", sqlConn, sqlTran);
                    break;
                /// ATTRIBUTES_CLASSES
                case nameof(Modules.AttributesClasses):
                    query = @"delete from wbz.attributes where class_id=@id;
						delete from wbz.attributes_classes where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto klasę atrybutu: {name}", sqlConn, sqlTran);
                    break;
                /// CONTRACTORS
                case nameof(Modules.Contractors):
                    query = @"delete from wbz.contractors where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto firmę: {name}", sqlConn, sqlTran);
                    break;
                /// DISTRIBUTIONS
                case nameof(Modules.Distributions):
                    if (instanceID != 0)
                    {
                        using var sqlCmd_Dis = new NpgsqlCommand(@"select status from wbz.distributions where id=@id", sqlConn, sqlTran);
                        sqlCmd_Dis.Parameters.AddWithValue("id", instanceID);
                        oldstatus = Convert.ToInt32(sqlCmd_Dis.ExecuteScalar());
                    }
                    query = @"delete from wbz.distributions_positions where distribution_id=@id;
						delete from wbz.distributions where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto dystrybucję: {name}", sqlConn, sqlTran);
                    break;
                /// DOCUMENTS
                case nameof(Modules.Documents):
                    if (instanceID != 0)
                    {
                        using var sqlCmd_Doc = new NpgsqlCommand(@"select status from wbz.documents where id=@id", sqlConn, sqlTran);
                        sqlCmd_Doc.Parameters.AddWithValue("id", instanceID);
                        oldstatus = Convert.ToInt32(sqlCmd_Doc.ExecuteScalar());
                    }
                    query = @"delete from wbz.documents_positions WHERE document_id=@id;
						delete from wbz.documents WHERE id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto dokument: {name}", sqlConn, sqlTran);
                    break;
                /// EMPLOYEES
                case nameof(Modules.Employees):
                    query = @"delete from wbz.employees where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto pracownika: {name}", sqlConn, sqlTran);
                    break;
                /// FAMILIES
                case nameof(Modules.Families):
                    query = @"delete from wbz.families where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto rodzinę: {name}", sqlConn, sqlTran);
                    break;
                /// GROUPS
                case nameof(Modules._submodules.Groups):
                    query = @"delete from wbz.groups where id=@id;
						delete from wbz.groups where owner_id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto grupę: {name}", sqlConn, sqlTran);
                    break;
                /// ICONS
                case nameof(Modules.Icons):
                    query = @"delete from wbz.icons where id=@id;";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto ikonę: {name}", sqlConn, sqlTran);
                    break;
                /// LOGS
                case nameof(Modules.Logs):
                    query = @"delete from wbz.logs where id=@id";
                    break;
                /// STORES
                case nameof(Modules.Stores):
                    query = @"delete from wbz.stores_articles where store_id=@id;
						delete from wbz.stores where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto magazyn: {name}", sqlConn, sqlTran);
                    break;
                /// USERS
                case nameof(Modules.Users):
                    query = @"delete from wbz.users where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto użytkownika: {name}", sqlConn, sqlTran);
                    break;
                /// VEHICLES
                case nameof(Modules.Vehicles):
                    query = @"delete from wbz.vehicles where id=@id";
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto pojazd: {name}", sqlConn, sqlTran);
                    break;
                default:
                    throw new NotImplementedException();
            }
            using (var sqlCmd = new NpgsqlCommand(query, sqlConn))
            {
                sqlCmd.Parameters.AddWithValue("id", instanceID);
                sqlCmd.ExecuteNonQuery();
            }
            ClearObject(module, instanceID, sqlConn, sqlTran);

            /// update articles quantity
            if (oldstatus > 0)
            {
                /// DISTRIBUTIONS
                if (module.Name == nameof(Modules.Distributions))
                {
                    var families = GetDistributionPositions(instanceID);
                    foreach (var family in families)
                        foreach (DataRow pos in family.Positions.Rows)
                            ChangeArticleQuantity((int)pos["store_id"], (int)pos["article_id"], (double)pos["quantity"], (string)pos["measure"], false, sqlConn, sqlTran);
                }
                /// DOCUMENTS
                else if (module.Name == nameof(Modules.Documents))
                {
                    var document = GetInstance<M_Document>(Config.GetModule(nameof(Modules.Documents)), instanceID);
                    var positions = GetInstancePositions(Config.GetModule(nameof(Modules.Documents)), instanceID);
                    foreach (DataRow pos in positions.Rows)
                        ChangeArticleQuantity(document.StoreID, (int)pos["article_id"], -(double)pos["quantity"], (string)pos["measure"], false, sqlConn, sqlTran);
                }
            }

            sqlTran.Commit();

            return result;
        }
        #endregion

        #region additional
        /// <summary>
        /// Pobiera wartość podanej właściwości z bazy danych
        /// </summary>
        /// <param name="property">Nazwa właściwości z tabeli wbz.config</param>
        /// <param name="def">Wartość domyślna jeśli zapis w tabeli wbz.config nie istnieje</param>
        internal static string GetPropertyValue(string property, string def = null)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return def;

            string result = null;

            try
            {
                using (var sqlConn = ConnOpenedWBZ)
                using (var sqlCmd = new NpgsqlCommand(@"select coalesce((select value from wbz.config where property=@property), '')", sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("property", property);
                    result = sqlCmd.ExecuteScalar().ToString();
                }

                if (result == string.Empty && def != null)
                {
                    SetPropertyValue(property, def);
                    result = def;
                }
            }
            catch
            {
                result = def;
            }

            return result;
        }

        /// <summary>
        /// Ustawia wartość podanej właściwości z bazy danych
        /// </summary>
        /// <param name="property">Nazwa właściwości z tabeli wbz.config</param>
        /// <param name="value">Wartość właściwości z tabeli wbz.config</param>
        internal static bool SetPropertyValue(string property, string value)
        {
            using var sqlConn = ConnOpenedWBZ;
            var query = @"insert into wbz.config (property, value)
				values (@property, @value)
				on conflict(property) do
				update set value=@value";
            using var sqlCmd = new NpgsqlCommand(query, sqlConn);
            sqlCmd.Parameters.AddWithValue("property", property);
            sqlCmd.Parameters.AddWithValue("value", value);
            sqlCmd.ExecuteNonQuery();

            return true;
        }

        /// <summary>
        /// Usuwa wszystkie kontakty, atrybuty i załączniki przypisane do obiektu
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        internal static bool ClearObject(MV module, int instanceID, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
        {
            using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.attachments
				where module_alias=@module_alias and instance_id=@instance_id", sqlConn, sqlTran))
            {
                sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
                sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
                sqlCmd.ExecuteNonQuery();
            }
            using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes
				where class_id in (select id from wbz.attributes_classes where module_alias=@module_alias) and instance_id=@instance_id", sqlConn, sqlTran))
            {
                sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
                sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
                sqlCmd.ExecuteNonQuery();
            }
            using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.contacts
				where module_alias=@module_alias and instance_id=@instance_id", sqlConn, sqlTran))
            {
                sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
                sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
                sqlCmd.ExecuteNonQuery();
            }
            using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.groups
				where module_alias=@module_alias and instance_id=@instance_id", sqlConn, sqlTran))
            {
                sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
                sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
                sqlCmd.ExecuteNonQuery();
            }

            return true;
        }
        internal static bool ClearObject(MV module, int instanceID)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlTran = sqlConn.BeginTransaction();
            ClearObject(module, instanceID, sqlConn, sqlTran);
            sqlTran.Commit();
            return true;
        }

        /// <summary>
        /// Pobiera zawartość załącznika o podanych parametrach
        /// </summary>
        /// <param name="id">ID załącznika</param>
        internal static byte[] GetAttachmentFile(int id)
        {
            using var sqlConn = ConnOpenedWBZ;
            var sqlCmd = new NpgsqlCommand(@"select content
				from wbz.attachments
				where id=@id", sqlConn);
            sqlCmd.Parameters.AddWithValue("id", id);
            return (byte[])sqlCmd.ExecuteScalar();
        }

        /// <summary>
        /// Zapisuje załącznik o podanych parametrach
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="name">Nazwa załącznika</param>
        /// <param name="path">Ścieżka do pliku</param>
        /// <param name="content">Plik</param>
        /// <param name="comment">Komentarz</param>
        internal static int SetAttachment(MV module, int instanceID, string name, string path, byte[] content, string comment)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlTran = sqlConn.BeginTransaction();
            var query = @"insert into wbz.attachments (user_id, module_alias, instance_id,
					name, format, path, content, comment)
				values (@user_id, @module_alias, @instance_id,
					@name, @format, @path, @content, @comment) returning id";
            var sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran);
            sqlCmd.Parameters.AddWithValue("user_id", Config.User.ID);
            sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
            sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
            sqlCmd.Parameters.AddWithValue("name", name);
            sqlCmd.Parameters.AddWithValue("format", path.Split('.').Last());
            sqlCmd.Parameters.AddWithValue("path", path);
            sqlCmd.Parameters.AddWithValue("size", content.Length);
            sqlCmd.Parameters.AddWithValue("content", content);
            sqlCmd.Parameters.AddWithValue("comment", comment);
            int result = Convert.ToInt32(sqlCmd.ExecuteScalar());

            SetLog(Config.User.ID, module, instanceID, $"Dodano załącznik {name}.", sqlConn, sqlTran);

            sqlTran.Commit();

            return result;
        }

        /// <summary>
        /// Pobiera atrybuty dla podanego modułu i obiektu
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="filter">Filtr SQL</param>
        internal static List<M_Attribute> ListAttributes(MV module, int instanceID, string filter = null)
        {
            var result = new List<M_Attribute>();

            using var sqlConn = ConnOpenedWBZ;
            var query = @"select a.id, ac.module_alias, @instance_id as instance_id,
					ac.id as class_id, ac.name, ac.type, ac.is_required, ac.icon_id, a.value as value
				from wbz.attributes_classes ac
				left join wbz.attributes a
					on ac.id=a.class_id and a.instance_id=@instance_id
				where ac.module_alias=@module_alias and @filter
				order by ac.name";
            var sqlCmd = new NpgsqlCommand(query, sqlConn);
            sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
            sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
            sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
            using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
            var dt = new DataTable();
            sqlDA.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                M_Attribute c = new M_Attribute()
                {
                    ID = !Convert.IsDBNull(row["id"]) ? (long)row["id"] : 0,
                    Class = new M_AttributeClass()
                    {
                        ID = !Convert.IsDBNull(row["class_id"]) ? (int)row["class_id"] : 0,
                        Module = Config.ListModules.Find(x => x.Alias == (!Convert.IsDBNull(row["module_alias"]) ? (string)row["module_alias"] : string.Empty)),
                        Name = !Convert.IsDBNull(row["name"]) ? (string)row["name"] : string.Empty,
                        Type = !Convert.IsDBNull(row["type"]) ? (string)row["type"] : string.Empty,
                        IsRequired = !Convert.IsDBNull(row["is_required"]) && (bool)row["is_required"],
                        IconID = !Convert.IsDBNull(row["icon_id"]) ? (int)row["icon_id"] : 0,
                        Values = GetInstancePositions(Config.GetModule(nameof(Modules.AttributesClasses)), !Convert.IsDBNull(row["class_id"]) ? (int)row["class_id"] : 0)
                    },
                    InstanceID = !Convert.IsDBNull(row["instance_id"]) ? (int)row["instance_id"] : 0,
                    Value = !Convert.IsDBNull(row["value"]) ? (string)row["value"] : string.Empty
                };
                result.Add(c);
            }

            return result;
        }

        /// <summary>
        /// Ustawia dane o atrybucie
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="attribute">Atrybut</param>
        internal static bool UpdateAttribute(MV module, M_Attribute attribute)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlTran = sqlConn.BeginTransaction();
            ///add
            if (attribute.ID == 0)
            {
                var sqlCmd = new NpgsqlCommand(@"insert into wbz.attributes (instance_id, class_id, value)
					values (@instance_id, @class_id, @value)", sqlConn, sqlTran);
                sqlCmd.Parameters.AddWithValue("instance_id", attribute.InstanceID);
                sqlCmd.Parameters.AddWithValue("class_id", attribute.Class.ID);
                sqlCmd.Parameters.AddWithValue("value", attribute.Value);
                sqlCmd.ExecuteNonQuery();
                SetLog(Config.User.ID, module, attribute.InstanceID, $"Dodano atrybut {attribute.Class.Name}.", sqlConn, sqlTran);
            }
            ///edit
            else if (!string.IsNullOrEmpty(attribute.Value))
            {
                var sqlCmd = new NpgsqlCommand(@"update wbz.attributes
					set instance_id=@instance_id, class_id=@class_id, value=@value
					where id=@id", sqlConn, sqlTran);
                sqlCmd.Parameters.AddWithValue("id", attribute.ID);
                sqlCmd.Parameters.AddWithValue("instance_id", attribute.InstanceID);
                sqlCmd.Parameters.AddWithValue("class_id", attribute.Class.ID);
                sqlCmd.Parameters.AddWithValue("value", attribute.Value);
                sqlCmd.ExecuteNonQuery();
                SetLog(Config.User.ID, module, attribute.InstanceID, $"Edytowano atrybut {attribute.Class.Name}.", sqlConn, sqlTran);
            }
            ///delete
            else
            {
                var sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes
					where id=@id", sqlConn, sqlTran);
                sqlCmd.Parameters.AddWithValue("id", attribute.ID);
                sqlCmd.ExecuteNonQuery();
                SetLog(Config.User.ID, module, attribute.InstanceID, $"Usunięto atrybut {attribute.Class.Name}.", sqlConn, sqlTran);
            }

            sqlTran.Commit();

            return true;
        }

        /// <summary>
        /// Pobiera kontakty dla podanego modułu i obiektu
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="filter">Filtr SQL</param>
        internal static DataTable ListContacts(MV module, int instanceID, string filter = null)
        {
            var result = new DataTable();

            using var sqlConn = ConnOpenedWBZ;
            var query = @"select c.id, c.module_alias, c.instance_id, c.email, c.phone, c.forename, c.lastname, c.is_default, c.is_archival
					from wbz.contacts c
					where c.module_alias=@module_alias and c.instance_id=@instance_id and @filter";
            var sqlCmd = new NpgsqlCommand(query, sqlConn);
            sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
            sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
            sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
            using var sqlDA = new NpgsqlDataAdapter(sqlCmd);
            sqlDA.Fill(result);
            result.Columns["is_default"].DefaultValue = false;
            result.Columns["is_archival"].DefaultValue = false;

            return result;
        }

        /// <summary>
        /// Ustawia dane o kontaktach
        /// </summary>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="contacts">Tabela kontaktów</param>
        internal static bool UpdateContacts(MV module, int instanceID, DataTable contacts)
        {
            using var sqlConn = ConnOpenedWBZ;
            using var sqlTran = sqlConn.BeginTransaction();
            NpgsqlCommand sqlCmd;

            ///contacts
            foreach (DataRow contact in contacts.Rows)
            {
                ///add
                if (contact.RowState == DataRowState.Added)
                {
                    sqlCmd = new NpgsqlCommand(@"insert into wbz.contacts (module_alias, instance_id, email, phone, forename, lastname, is_default, is_archival)
							values (@module_alias, @instance_id, @email, @phone, @forename, @lastname, @is_default, @is_archival)", sqlConn, sqlTran);
                    sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
                    sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
                    sqlCmd.Parameters.AddWithValue("email", contact["email"]);
                    sqlCmd.Parameters.AddWithValue("phone", contact["phone"]);
                    sqlCmd.Parameters.AddWithValue("forename", contact["forename"]);
                    sqlCmd.Parameters.AddWithValue("lastname", contact["lastname"]);
                    sqlCmd.Parameters.AddWithValue("is_default", contact["is_default"]);
                    sqlCmd.Parameters.AddWithValue("is_archival", contact["is_archival"]);
                    sqlCmd.ExecuteNonQuery();
                    SetLog(Config.User.ID, module, instanceID, $"Dodano kontakt {contact["forename"]} {contact["lastname"]}.", sqlConn, sqlTran);
                }
                ///edit
                else if (contact.RowState == DataRowState.Modified)
                {
                    sqlCmd = new NpgsqlCommand(@"update wbz.contacts
							set email=@email, phone=@phone, forename=@forename, lastname=@lastname, is_default=@default, is_archival=@is_archival
							where id=@id", sqlConn, sqlTran);
                    sqlCmd.Parameters.AddWithValue("id", contact["id", DataRowVersion.Original]);
                    sqlCmd.Parameters.AddWithValue("email", contact["email"]);
                    sqlCmd.Parameters.AddWithValue("phone", contact["phone"]);
                    sqlCmd.Parameters.AddWithValue("forename", contact["forename"]);
                    sqlCmd.Parameters.AddWithValue("lastname", contact["lastname"]);
                    sqlCmd.Parameters.AddWithValue("is_default", contact["is_default"]);
                    sqlCmd.Parameters.AddWithValue("is_archival", contact["is_archival"]);
                    sqlCmd.ExecuteNonQuery();
                    SetLog(Config.User.ID, module, instanceID, $"Edytowano kontakt {contact["forename", DataRowVersion.Original]} {contact["lastname", DataRowVersion.Original]}.", sqlConn, sqlTran);
                }
                ///delete
                else if (contact.RowState == DataRowState.Deleted)
                {
                    sqlCmd = new NpgsqlCommand(@"delete from wbz.contacts
							where id=@id", sqlConn, sqlTran);
                    sqlCmd.Parameters.AddWithValue("id", contact["id", DataRowVersion.Original]);
                    sqlCmd.ExecuteNonQuery();
                    SetLog(Config.User.ID, module, instanceID, $"Usunięto kontakt {contact["forename", DataRowVersion.Original]} {contact["lastname", DataRowVersion.Original]}.", sqlConn, sqlTran);
                }
            }

            sqlTran.Commit();

            return true;
        }

        /// <summary>
        /// Zapisuje log o podanych parametrach
        /// </summary>
        /// <param name="userID">ID użytkownika</param>
        /// <param name="module">Moduł</param>
        /// <param name="instanceID">ID instancji</param>
        /// <param name="content">Treść logu</param>
        internal static bool SetLog(int userID, MV module, int instanceID, string content, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
        {
            if (Config.Logs_Enabled != "1")
                return true;

            using var sqlCmd = new NpgsqlCommand(@"insert into wbz.logs (user_id, module_alias, instance_id, type, content)
				values (@user_id, @module_alias, @instance_id, 1, @content)", sqlConn, sqlTran);
            sqlCmd.Parameters.AddWithValue("user_id", userID);
            sqlCmd.Parameters.AddWithValue("module_alias", module.Alias);
            sqlCmd.Parameters.AddWithValue("instance_id", instanceID);
            sqlCmd.Parameters.AddWithValue("content", content);
            sqlCmd.ExecuteNonQuery();

            return true;
        }
        #endregion
    }
}
