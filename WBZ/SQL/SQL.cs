using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Windows;
using WBZ.Models;
using WBZ.Globals;
using WBZ.Controls;
using System.Collections.ObjectModel;

namespace WBZ
{
	internal static class SQL
	{
		internal static string connWBZ = null; ///przypisywanie połączenia w oknie logowania
		internal static NpgsqlConnection connOpenedWBZ => OpenConnection(new NpgsqlConnection(connWBZ));

		/// <summary>
		/// Podmienia połączenie do bazy na nowe
		/// </summary>
		/// <param name="server">Nazwa serwera</param>
		/// <param name="port">Port serwera</param>
		/// <param name="database">Nazwa bazy danych</param>
		/// <param name="userID">Nazwa użytkownika</param>
		/// <param name="password">Hasło użytkownika</param>
		internal static string MakeConnString(string server, int port, string database, string userID, string password)
		{
			return $"Server={server};Port={port};Database={database};User Id={userID};Password={password};";
		}

		/// <summary>
		/// Otwiera przekazane połączenie i zwraca je.
		/// </summary>
		/// <param name="sqlConn">Połączenie do otworzenia.</param>
		/// <returns>Otwarte połączenie.</returns>
		internal static NpgsqlConnection OpenConnection(NpgsqlConnection sqlConn)
		{
			sqlConn.Open();
			return sqlConn;
		}

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
			
			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					DataTable user = new DataTable(), perms = new DataTable();

					var sqlCmd = new NpgsqlCommand(@"select id, username, email, phone, forename, lastname, blocked, archival
						from wbz.users
						where (lower(username)=lower(@login) or lower(email)=lower(@login))
							and password=@password", sqlConn);
					sqlCmd.Parameters.AddWithValue("login", login);
					sqlCmd.Parameters.AddWithValue("password", password);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(user);
					}

					if (user.Rows.Count > 0)
					{
						Global.User = user.DataTableToList<M_User>()[0];
						if (Global.User.Blocked)
							MessageBox.Show("Użytkownik o podanym loginie jest zablokowany.");
						else
						{
							sqlCmd = new NpgsqlCommand(@"select user, perm
								from wbz.users_permissions
								where ""user""=@id", sqlConn);
							sqlCmd.Parameters.AddWithValue("id", Global.User.ID);
							using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
							{
								sqlDA.Fill(perms);
							}

							foreach (DataRow perm in perms.Rows)
								Global.User.Perms.Add(perm["perm"].ToString());

							result = true;
						}
					}
					else
						MessageBox.Show("Brak użytkownika w bazie lub złe dane logowania.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Tworzy konto użytkownika w bazie o podanych parametrach
		/// </summary>
		/// <param name="email">Adres e-mail</param>
		/// <param name="username">Nazwa użytkownika</param>
		/// <param name="password">Hasło do konta</param>
		/// <param name="admin">Czy nadać uprawnienia administracyjne</param>
		/// <returns></returns>
		internal static bool Register(string email, string username, string password, bool admin)
		{
			bool result = false;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					var sqlCmd = new NpgsqlCommand(@"insert into wbz.users (email, username, password)
						values (@email, @username, @password) returning id", sqlConn, sqlTran);
					sqlCmd.Parameters.AddWithValue("email", email);
					sqlCmd.Parameters.AddWithValue("username", username);
					sqlCmd.Parameters.AddWithValue("password", Global.sha256(password));
					int id = (int)sqlCmd.ExecuteScalar();

					///permissions
					sqlCmd = new NpgsqlCommand(@"insert into wbz.users_permissions (""user"", perm)
							values (@id, 'admin'),
								(@id, 'users_preview'),
								(@id, 'users_save'),
								(@id, 'users_delete')", sqlConn, sqlTran);
					sqlCmd.Parameters.AddWithValue("id", id);
					sqlCmd.ExecuteNonQuery();

					sqlTran.Commit();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Generuje nowe hasło dla konta powiązanego z podanym e-mailem i wysyła na pocztę
		/// </summary>
		/// <param name="email">Adres e-mail powiązanego konta</param>
		/// <returns></returns>
		internal static DataRow GenerateNewPasswordForAccount(string email)
		{
			var dt = new DataTable();
			DataRow result = null;
			Random rnd = new Random();
			string newpass = rnd.NextDouble().ToString().Substring(2, 6);

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					var sqlCmd = new NpgsqlCommand(@"update wbz.users
						set password=@newpass
						where email=@email", sqlConn, sqlTran);
					sqlCmd.Parameters.AddWithValue("newpass", Global.sha256(newpass));
					sqlCmd.Parameters.AddWithValue("email", email);
					sqlCmd.ExecuteNonQuery();

					sqlCmd = new NpgsqlCommand(@"select u.username, u.password
						from wbz.users u
						where email=@email", sqlConn, sqlTran);
					sqlCmd.Parameters.AddWithValue("email", email);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(dt);
						result = dt.Rows[0];
						dt.Rows[0][1] = newpass;
					}

					sqlTran.Commit();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		#endregion

		/// <summary>
		/// Pobiera uprawnienia użytkownika
		/// </summary>
		/// <param name="id">ID użytkownika</param>
		internal static List<string> GetUserPerms(int id)
		{
			var result = new List<string>();

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select up.perm
						from wbz.users_permissions up
						where ""user""=@id", sqlConn);
					sqlCmd.Parameters.AddWithValue("id", id);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						foreach (DataRow row in dt.Rows)
							result.Add(row["perm"].ToString());
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}

		/// <summary>
		/// Usunięcie dokumentu
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteDocument(int id)
		{
			bool result = false;
			int oldstatus = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					NpgsqlCommand sqlCmd;

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						///take oldstatus if invoice exists
						if (id != 0)
						{
							sqlCmd = new NpgsqlCommand(@"select status from wbz.documents where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", id);
							oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
						}

						sqlCmd = new NpgsqlCommand(@"delete from wbz.documents_positions WHERE document=@id;
							delete from wbz.documents WHERE id=@id", sqlConn, sqlTran);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						///update articles amounts
						if (oldstatus > 0)
						{
							var document = GetInstance<M_Document>("documents", id);
							var positions = GetInstancePositions("documents", id);
							foreach (DataRow pos in positions.Rows)
								ChangeArticleAmount(document.Store, (int)pos["article"], -(double)pos["amount"], (string)pos["measure"], false, sqlConn, sqlTran);
						}

						ClearObject("documents", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "documents", id, $"Usunięto dokument.", sqlTran);

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}

		/// <summary>
		/// Pobiera przelicznik głównej jednostki miary towaru
		/// </summary>
		/// <param name="article">ID towaru</param>
		/// <returns></returns>
		internal static double GetArtDefMeaCon(int article)
		{
			double result = 0;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select wbz.artdefmeacon(@article)", sqlConn);
					sqlCmd.Parameters.AddWithValue("article", article);
					result = Convert.ToDouble(sqlCmd.ExecuteScalar());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}

		/// <summary>
		/// Ustawia stany towaru
		/// </summary>
		/// <param name="store">ID magazynu</param>
		/// <param name="article">ID towaru</param>
		/// <param name="amount">Ilość towaru (kg)</param>
		/// <param name="measure">Jednostka miary</param>
		/// <param name="reserved">Czy rezerwacja</param>
		/// <param name="sqlConn">Połączenie SQL</param>
		/// <param name="sqlTran">Transakcja SQL</param>
		/// <returns></returns>
		internal static bool ChangeArticleAmount(int store, int article, double amount, string measure, bool reserved, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
		{
			bool result = false;

			try
			{
				var sqlCmd = new NpgsqlCommand(@"select case
					when exists(select from wbz.stores_articles where store=@store and article=@article) then true
					else false end", sqlConn, sqlTran);
				sqlCmd.Parameters.AddWithValue("store", store);
				sqlCmd.Parameters.AddWithValue("article", article);
				bool exists = Convert.ToBoolean(sqlCmd.ExecuteScalar());

				if (!reserved)
				{
					if (exists)
						sqlCmd = new NpgsqlCommand(@"update wbz.stores_articles
							set amount=amount+(@amount * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1))
							where store=@store and article=@article", sqlConn, sqlTran);
					else
						sqlCmd = new NpgsqlCommand(@"insert into wbz.stores_articles (store, article, amount, reserved)
							values (@store, @article, (@amount * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1)), 0)", sqlConn, sqlTran);
				}
				else
				{
					if (exists)
						sqlCmd = new NpgsqlCommand(@"update wbz.stores_articles
							set reserved=reserved+(@amount * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1))
							where store=@store and article=@article", sqlConn, sqlTran);
					else
						sqlCmd = new NpgsqlCommand(@"insert into wbz.stores_articles (store, article, amount, reserved)
							values (@store, @article, 0, (@amount * coalesce(nullif((select converter from wbz.articles_measures where article=@article and name=@measure limit 1),0),1)))", sqlConn, sqlTran);
				}
				sqlCmd.Parameters.AddWithValue("store", store);
				sqlCmd.Parameters.AddWithValue("article", article);
				sqlCmd.Parameters.AddWithValue("amount", amount);
				sqlCmd.Parameters.AddWithValue("measure", measure);
				sqlCmd.ExecuteNonQuery();

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
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
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select 0 as id, 0 as position, 0 as store, '' as storename,
						0 as article, '' as articlename, 0.0 as amount, '' as measure", sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
						result.Rows.Clear();
					}
				}
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
				using (var sqlConn = connOpenedWBZ)
				{
					NpgsqlCommand sqlCmd;

					///add
					if (id == 0)
						dt = GetDistributionPositionsFormatting();
					///edit
					else
					{
						sqlCmd = new NpgsqlCommand(@"select id, position, family, (select lastname from wbz.families where id=dp.family) as familyname, members,
								store, (select name from wbz.stores where id=dp.store) as storename,
								article, (select name from wbz.articles where id=dp.article) as articlename,
								amount / wbz.ArtDefMeaCon(dp.article) as amount, coalesce(nullif(wbz.ArtDefMeaNam(dp.article),''), 'kg') as measure, status
							from wbz.distributions_positions dp
							where distribution=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
						{
							sqlDA.Fill(dt);
						}
					}
				}

				///przypisywanie towarów do rodzin
				foreach (DataRow row in dt.Rows)
				{
					var family = result.FirstOrDefault(x => x.Family == (int)row["family"]);
					if (family == null)
					{
						family = new M_DistributionFamily()
						{
							Family = (int)row["family"],
							FamilyName = (string)row["familyname"],
							Members = Convert.ToInt16(row["members"]),
							Status = Convert.ToInt16(row["status"])
						};
						result.Add(family);
						family = result.FirstOrDefault(x => x.Family == (int)row["family"]);
					}

					var position = family.Positions.NewRow();

					position["id"] = row["id"];
					position["position"] = row["position"];
					position["store"] = row["store"];
					position["storename"] = row["storename"];
					position["article"] = row["article"];
					position["articlename"] = row["articlename"];
					position["amount"] = row["amount"];
					position["measure"] = row["measure"];

					family.Positions.Rows.Add(position);
					family.Positions.Rows[family.Positions.Rows.Count - 1].AcceptChanges();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}

		/// <summary>
		/// Ustawia dane o dystrybucji
		/// </summary>
		/// <param name="distribution">Klasa dystrybucji</param>
		internal static bool SetDistribution(M_Distribution distribution)
		{
			bool result = false;
			int ID = distribution.ID;
			int oldstatus = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///take oldstatus if distribution exists
						if (ID != 0)
						{
							sqlCmd = new NpgsqlCommand(@"select status from wbz.distributions where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", ID);
							oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
						}

						///add
						if (ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.distributions (name, datereal, status, archival, comment, icon)
								values (@name, @datereal, @status, @archival, @comment, @icon) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.distributions
								set name=@name, datereal=@datereal, status=@status, archival=@archival, comment=@comment, icon=@icon
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("name", distribution.Name);
						sqlCmd.Parameters.AddWithValue("datereal", distribution.DateReal);
						sqlCmd.Parameters.AddWithValue("status", distribution.Status);
						sqlCmd.Parameters.AddWithValue("archival", distribution.Archival);
						sqlCmd.Parameters.AddWithValue("comment", distribution.Comment);
						sqlCmd.Parameters.AddWithValue("icon", (object)distribution.Icon ?? DBNull.Value);

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "distributions", ID, $"Utworzono dystrybucję.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "distributions", ID, $"Edytowano dystrybucję.", sqlTran);
						}

						///positions
						foreach (var family in distribution.Families)
						foreach (DataRow position in family.Positions.Rows)
						{
							///add
							if (position.RowState == DataRowState.Added)
							{
								sqlCmd = new NpgsqlCommand(@"insert into wbz.distributions_positions (distribution, position, family, members, store, article, amount, status)
									values (@distribution, @position, @family, @members, @store, @article, (@amount * wbz.ArtDefMeaCon(@article)), @status)", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("distribution", ID);
								sqlCmd.Parameters.AddWithValue("position", position["position"]);
								sqlCmd.Parameters.AddWithValue("family", family.Family);
								sqlCmd.Parameters.AddWithValue("members", family.Members);
								sqlCmd.Parameters.AddWithValue("store", position["store"]);
								sqlCmd.Parameters.AddWithValue("article", position["article"]);
								sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
								sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
								sqlCmd.Parameters.AddWithValue("status", family.Status);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "distributions", ID, $"Dodano pozycję {position["position"]}.", sqlTran);
							}
							///edit
							else if (position.RowState == DataRowState.Modified)
							{
								sqlCmd = new NpgsqlCommand(@"update wbz.distributions_positions
									set position=@position, family=@family, members=@members, store=@store, article=@article, amount=(@amount * wbz.ArtDefMeaCon(@article)), status=@status
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
								sqlCmd.Parameters.AddWithValue("position", position["position"]);
								sqlCmd.Parameters.AddWithValue("family", family.Family);
								sqlCmd.Parameters.AddWithValue("members", family.Members);
								sqlCmd.Parameters.AddWithValue("store", position["store"]);
								sqlCmd.Parameters.AddWithValue("article", position["article"]);
								sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
								sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
								sqlCmd.Parameters.AddWithValue("status", family.Status);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "distributions", ID, $"Edytowano pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
							}
							///delete
							else if (position.RowState == DataRowState.Deleted)
							{
								sqlCmd = new NpgsqlCommand(@"delete from wbz.distributions_positions
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "distributions", ID, $"Usunięto pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
							}

							///update articles amounts
							if (oldstatus != distribution.Status)
							{
								if (oldstatus <= 0 && distribution.Status > 0)
									ChangeArticleAmount((int)position["store"], (int)position["article"], -(double)position["amount"], (string)position["measure"], false, sqlConn, sqlTran);
								else if (oldstatus > 0 && distribution.Status < 0)
									ChangeArticleAmount((int)position["store"], (int)position["article"], (double)position["amount"], (string)position["measure"], false, sqlConn, sqlTran);
							}
						}

						sqlTran.Commit();
						distribution.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}

		/// <summary>
		/// Usunięcie dystrybucji
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteDistribution(int id)
		{
			bool result = false;
			int oldstatus = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					NpgsqlCommand sqlCmd;

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						///take oldstatus if invoice exists
						if (id != 0)
						{
							sqlCmd = new NpgsqlCommand(@"select status from wbz.distributions where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", id);
							oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
						}

						sqlCmd = new NpgsqlCommand(@"delete from wbz.distributions_positions where distribution=@id;
							delete from wbz.distributions where id=@id", sqlConn, sqlTran);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						///update articles amounts
						if (oldstatus > 0)
						{
							var families = GetDistributionPositions(id);
							foreach (var family in families)
							foreach (DataRow pos in family.Positions.Rows)
								ChangeArticleAmount((int)pos["store"], (int)pos["article"], (double)pos["amount"], (string)pos["measure"], false, sqlConn, sqlTran);
						}

						ClearObject("distributions", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "distributions", id, $"Usunięto dystrybucję.", sqlTran);

						sqlTran.Commit();
					}

					sqlConn.Close();
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
				using (var sqlConn = connOpenedWBZ)
				{
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
								(SELECT sum(amount) WHERE extract(month from dateissue) =  1) as m_01,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  2) as m_02,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  3) as m_03,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  4) as m_04,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  5) as m_05,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  6) as m_06,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  7) as m_07,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  8) as m_08,
								(SELECT sum(amount) WHERE extract(month from dateissue) =  9) as m_09,
								(SELECT sum(amount) WHERE extract(month from dateissue) = 10) as m_10,
								(SELECT sum(amount) WHERE extract(month from dateissue) = 11) as m_11,
								(SELECT sum(amount) WHERE extract(month from dateissue) = 12) as m_12
							from wbz.documents i
							inner join wbz.documents_positions
								on document = i.id
							where status = 1
							group by extract(year from dateissue), dateissue
						) as q
						group by year", sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}
				}
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
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select coalesce(sum(amount),0)
						from wbz.documents i
						inner join wbz.documents_positions
							on document=i.id
						where status=1", sqlConn);
					result = Convert.ToDouble(sqlCmd.ExecuteScalar());
				}
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
		internal static DataTable GetDonationSumCompany(DateTime from, DateTime to)
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"SELECT DISTINCT (SELECT c.name FROM wbz.companies c WHERE c.id = i.company)
						FROM wbz.documents i 
						WHERE i.archival = false and (dateissue between '@from' and '@to')", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@from", from.ToString("yyyy.MM.dd"));
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@to", to.ToString("yyyy.MM.dd 23:59:59"));
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}
				}
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
		/// <param name="company">???</param>
		internal static DataRow GetDonationSumCompanyValue(DateTime date, string company)
		{
			var dt = new DataTable();
			DataRow result = null;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"SELECT 
						SUM((SELECT SUM(ip.amount) FROM wbz.documents_positions ip WHERE i.id = ip.document)) as amount,
						SUM((SELECT SUM(ip.cost) FROM wbz.documents_positions ip WHERE i.id = ip.document)) as cost
						FROM wbz.documents i join wbz.companies c ON (i.company = c.id)
						WHERE i.archival = false and i.dateissue = '" + date.ToString("yyyy.MM.dd") + @"' and c.name = @company and c.archival = false", sqlConn);
					//sqlCmd.Parameters.AddWithValue("date", date);
					sqlCmd.Parameters.AddWithValue("company", company);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(dt);
						result = dt.Rows[0];
					}
				}
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
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"SELECT DISTINCT to_char(i.dateissue,'dd.MM.yyyy') as day, i.dateissue
						FROM wbz.documents i
						WHERE i.archival = false and (i.dateissue between '@from' and '@to')
						ORDER BY dateissue asc", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@from", from.ToString("yyyy.MM.dd"));
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@to", to.ToString("yyyy.MM.dd 23:59:59"));
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}
				}
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
		private static void Error(string msg, Exception ex, string module, int instance, bool show = true, bool save = true)
        {
			try
			{
				var error = new M_Log()
				{
					ID = NewInstanceID(Global.Module.LOGS),
					Instance = instance,
					Module = module,
					Group = 2,
					User = Global.User.ID
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
					if (show)
						new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"{msg}:{Environment.NewLine}{error.Content}").ShowDialog();
					if (save)
						SetInstance(Global.Module.LOGS, error, Commands.Type.NEW);
				}
			}
			catch { }
		}
		#endregion

		#region modules
		/// <summary>
		/// Liczy ilość instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="filter">Filtr SQL</param>
		internal static int CountInstances(string module, string filter = "true")
		{
			var result = 0;
			string query;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					switch (module)
					{
						/// ARTICLES
						case Global.Module.ARTICLES:
							query = @"select count(distinct a.id)
								from wbz.articles a
								left join wbz.stores_articles sa
									on a.id=sa.article
								left join wbz.groups g
									on g.instance=a.id";
							break;
						/// ATTACHMENTS
						case Global.Module.ATTACHMENTS:
							query = @"select count(distinct a.id)
								from wbz.attachments a
								left join wbz.users u
									on a.""user"" = u.id";
							break;
						/// ATTRIBUTES_CLASSES
						case Global.Module.ATTRIBUTES_CLASSES:
							query = @"select count(distinct ac.id)
								from wbz.attributes_classes ac
								left join wbz.groups g
									on g.instance=ac.id";
							break;
						/// COMPANIES
						case Global.Module.COMPANIES:
							query = @"select count(distinct c.id)
								from wbz.companies c
								left join wbz.groups g
									on g.instance=c.id";
							break;
						/// DISTRIBUTIONS
						case Global.Module.DISTRIBUTIONS:
							query = @"select count(distinct d.id)
								from wbz.distributions d
								left join wbz.groups g
									on g.instance=d.id";
							break;
						/// DOCUMENTS
						case Global.Module.DOCUMENTS:
							query = @"select count(distinct d.id)
								from wbz.documents d
								left join wbz.documents_positions dp
									on dp.document=d.id
								left join wbz.companies c
									on c.id=d.company
								left join wbz.stores s
									on s.id=d.store
								left join wbz.groups g
									on g.instance=d.id";
							break;
						/// EMPLOYEES
						case Global.Module.EMPLOYEES:
							query = @"select count(distinct e.id)
								from wbz.employees e
								left join wbz.users u
									on u.id=e.""user""
								left join wbz.groups g
									on g.instance=e.id";
							break;
						/// GROUPS
						case Global.Module.GROUPS:
							query = @"select count(distinct g.id)
								from wbz.groups g";
							break;
						/// FAMILIES
						case Global.Module.FAMILIES:
							query = @"select count(distinct f.id)
								from wbz.families f
								left join wbz.groups g
									on g.instance=f.id";
							break;
						/// LOGS
						case Global.Module.LOGS:
							query = @"select count(distinct l.id)
								from wbz.logs l
								left join wbz.users u
									on l.""user"" = u.id";
							break;
						/// STORES
						case Global.Module.STORES:
							query = @"select count(distinct s.id)
								from wbz.stores s
								left join wbz.stores_articles sa
									on s.id = sa.store
								left join wbz.groups g
									on g.instance=s.id";
							break;
						/// USERS
						case Global.Module.USERS:
							query = @"select count(distinct u.id)
								from wbz.users u
								left join wbz.groups g
									on g.instance=u.id";
							break;
						default:
							throw new NotImplementedException();
					}

					using (var sqlCmd = new NpgsqlCommand(query, sqlConn))
					{
						sqlCmd.CommandText += $" where {filter}";
						result = Convert.ToInt32(sqlCmd.ExecuteScalar());
					}
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania liczby instancji", ex, module, 0);
			}

			return result;
		}
		/// <summary>
		/// Pobiera listę instancji do ComboBoxów (zazwyczaj ID i Name)
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="column">Kolumna z której będą wyświetlane nazwy</param>
		/// <param name="filter">Filtr SQL</param>
		internal static ObservableCollection<M_ComboValue> ComboInstances(string module, string column, string filter, bool allowEmpty)
		{
			var result = new ObservableCollection<M_ComboValue>();

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					using (var sqlDA = new NpgsqlDataAdapter($@"select id, {column} as name
						from wbz.{module}
						where {filter}
						order by 2 asc", sqlConn))
					{
						if (allowEmpty)
							sqlDA.SelectCommand.CommandText = "select 0 as id, '' as name union " + sqlDA.SelectCommand.CommandText;

						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<M_ComboValue>();
					}
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania wartości listy rozwijanej", ex, module, 0);
			}

			return result;
		}
		/// <summary>
		/// Pobiera listę instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="sort">Kolekcja sortowania</param>
		/// <param name="page">Strona listy rekordów</param>
		internal static ObservableCollection<T> ListInstances<T>(string module, string filter, StringCollection sort = null, int page = 0) where T : class, new()
		{
			var result = new ObservableCollection<T>();
			string query;

			try
			{
				if (sort == null)
					sort = new StringCollection() { "1", "false", "2", "false", "50" };

				using (var sqlConn = connOpenedWBZ)
				{
					switch (module)
					{
						/// ARTICLES
						case Global.Module.ARTICLES:
							query = @"select a.id, a.codename, a.name, a.ean, coalesce(nullif(wbz.ArtDefMeaNam(a.id),''), 'kg') as measure,
									coalesce(sum(sa.amount), 0) as amountraw, coalesce(sum(sa.amount) / wbz.ArtDefMeaCon(a.id), 0) as amount,
									coalesce(sum(sa.reserved), 0) as reservedraw, coalesce(sum(sa.reserved) / wbz.ArtDefMeaCon(a.id), 0) as reserved,
									a.archival, a.comment, a.icon
								from wbz.articles a
								left join wbz.stores_articles sa
									on a.id=sa.article
								left join lateral (select g.owner from wbz.groups g where g.instance=a.id fetch first 1 row only) as g
									on true";
							break;
						/// ATTACHMENTS
						case Global.Module.ATTACHMENTS:
							query = @"select a.id, a.""user"", a.module, a.instance, a.name, null as file
								from wbz.attachments a
								left join wbz.users u
									on a.""user"" = u.id";
							break;
						/// ATTRIBUTES_CLASSES
						case Global.Module.ATTRIBUTES_CLASSES:
							query = @"select ac.id as ""ID"", ac.module as ""Module"", ac.name as ""Name"", ac.type as ""Type"", ac.""values"" as ""Values"",
									ac.archival, ac.comment, ac.icon
								from wbz.attributes_classes ac
								left join lateral (select g.owner from wbz.groups g where g.instance=ac.id fetch first 1 row only) as g
									on true";
							break;
						/// COMPANIES
						case Global.Module.COMPANIES:
							query = @"select c.id, c.codename, c.name, c.branch, c.nip, c.regon, c.postcode, c.city, c.address,
									c.archival, c.comment, c.icon
								from wbz.companies c
								left join lateral (select g.owner from wbz.groups g where g.instance=c.id fetch first 1 row only) as g
									on true";
							break;
						/// DISTRIBUTIONS
						case Global.Module.DISTRIBUTIONS:
							query = @"select d.id, d.name, d.datereal, d.status,
									count(distinct dp.family) as familiescount, sum(members) as memberscount,
									count(dp.*) as positionscount, sum(dp.amount) as weight,
									d.archival, d.comment, d.icon
								from wbz.distributions d
								left join wbz.distributions_positions dp
									on dp.distribution=d.id
								left join lateral (select g.owner from wbz.groups g where g.instance=d.id fetch first 1 row only) as g
									on true";
							break;
						/// DOCUMENTS
						case Global.Module.DOCUMENTS:
							query = @"select d.id, d.name, d.store, s.name as storename, d.company, c.name as companyname,
									d.type, d.dateissue, d.status, count(dp.*) as positionscount, sum(dp.amount) as weight, sum(dp.cost) as cost,
									d.archival, d.comment, d.icon
								from wbz.documents d
								left join wbz.documents_positions dp
									on dp.document=d.id
								left join wbz.companies c
									on c.id=d.company
								left join wbz.stores s
									on s.id=d.store
								left join lateral (select g.owner from wbz.groups g where g.instance=d.id fetch first 1 row only) as g
									on true";
							break;
						/// EMPLOYEES
						case Global.Module.EMPLOYEES:
							query = @"select e.id, e.""user"", u.lastname || ' ' || u.forename as username,
									e.forename, e.lastname, e.department, e.position,
									e.email, e.phone, e.postcode, e.city, e.address,
									e.archival, e.comment, e.icon
								from wbz.employees e
								left join wbz.users u
									on u.id=e.""user""
								left join lateral (select g.owner from wbz.groups g where g.instance=e.id fetch first 1 row only) as g
									on true";
							break;
						/// GROUPS
						case Global.Module.GROUPS:
							query = @"select g.id, g.module, g.name, g.instance, g.owner,
									case when trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\') = '' then ''
										else concat(trim(concat(g1.name, '\', g2.name, '\', g3.name, '\', g4.name), '\'), '\') end as path,
									g.archival, g.comment, g.icon
								from wbz.groups g
								left join wbz.groups g4 on g4.id=g.owner
								left join wbz.groups g3 on g3.id=g4.owner
								left join wbz.groups g2 on g2.id=g3.owner
								left join wbz.groups g1 on g1.id=g2.owner";
							break;
						/// FAMILIES
						case Global.Module.FAMILIES:
							query = @"select f.id, f.declarant, f.lastname, f.members, f.postcode, f.city, f.address,
									f.status, f.c_sms, f.c_call, f.c_email, max(d.datereal) as donationlast, sum(dp.amount) as donationweight,
									f.archival, f.comment, f.icon
								from wbz.families f
								left join wbz.distributions_positions dp
									on f.id=dp.family
								left join wbz.distributions d
									on dp.distribution=d.id
								left join lateral (select g.owner from wbz.groups g where g.instance=f.id fetch first 1 row only) as g
									on true";
							break;
						/// LOGS
						case Global.Module.LOGS:
							query = @"select l.id, l.""user"", u.lastname || ' ' || u.forename as userfullname,
									l.module, l.instance, l.type as group, l.content, l.datetime
								from wbz.logs l
								left join wbz.users u
									on l.""user"" = u.id";
							break;
						/// STORES
						case Global.Module.STORES:
							query = @"select s.id, s.codename, s.name, s.postcode, s.city, s.address,
									coalesce(sum(amount),0) as amount, coalesce(sum(reserved),0) as reserved,
									s.archival, s.comment, s.icon
								from wbz.stores s
								left join wbz.stores_articles sa
									on s.id = sa.store
								left join lateral (select g.owner from wbz.groups g where g.instance=s.id fetch first 1 row only) as g
									on true";
							break;
						/// USERS
						case Global.Module.USERS:
							query = @"select u.id, u.username, '' as newpass, u.forename, u.lastname,
									u.email, u.phone, u.blocked, u.archival
								from wbz.users u
								left join lateral (select g.owner from wbz.groups g where g.instance=u.id fetch first 1 row only) as g
									on true";
							break;
						default:
							throw new NotImplementedException();
					}
					using (var sqlDA = new NpgsqlDataAdapter(query, sqlConn))
					{
						sqlDA.SelectCommand.CommandText += $" where {filter}";
						if (module.In(Global.Module.ARTICLES, Global.Module.DISTRIBUTIONS, Global.Module.FAMILIES, Global.Module.STORES)) sqlDA.SelectCommand.CommandText += $" group by {module[0]}.id";
						if (module.In(Global.Module.DOCUMENTS)) sqlDA.SelectCommand.CommandText += $" group by d.id, c.id, s.id";
						sqlDA.SelectCommand.CommandText += $" order by {sort[0]} {(Convert.ToBoolean(sort[1]) ? "desc" : "asc")}, {sort[2]} {(Convert.ToBoolean(sort[3]) ? "desc" : "asc")}";
						sqlDA.SelectCommand.CommandText += $" limit {sort[4]} offset {Convert.ToInt32(sort[4]) * page}";

						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<T>();
					}
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania listy", ex, module, 0);
			}
			
			return result;
		}
		/// <summary>
		/// Pobiera dane o instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="id">ID instancji</param>
		internal static T GetInstance<T>(string module, int id) where T : class, new()
		{
			try
			{
				return ListInstances<T>(module, $"{module[0]}.id={id}")[0];
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania instancji", ex, module, id);
			}
			return default(T);
		}
		/// <summary>
		/// Pobiera listę pozycji instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="id">ID instancji</param>
		internal static DataTable GetInstancePositions(string module, int id)
		{
			DataTable result = new DataTable();
			string query;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					switch (module)
					{
						/// ARTICLES
						case Global.Module.ARTICLES:
							query = @"select am.id, am.name, am.converter, am.""default"",
									sa.amount / coalesce(nullif(am.converter,0),1) as amount, sa.reserved / coalesce(nullif(am.converter,0),1) as reserved
								from wbz.articles a
								inner join wbz.articles_measures am
									on a.id = am.article
								left join wbz.stores_articles sa
									on a.id = sa.article
								where a.id=@id";
							break;
						/// ATTRIBUTES_CLASSES
						case Global.Module.ATTRIBUTES_CLASSES:
							query = @"select id, value, archival
								from wbz.attributes_values av
								where class=@id";
							break;
						/// DISTRIBUTIONS
						case Global.Module.DISTRIBUTIONS:
							query = @"";
							break;
						/// DOCUMENTS
						case Global.Module.DOCUMENTS:
							query = @"select id, position, article, (select name from wbz.articles where id=dp.article) as articlename,
									amount / wbz.ArtDefMeaCon(dp.article) as amount, coalesce(nullif(wbz.ArtDefMeaNam(dp.article),''), 'kg') as measure, cost
								from wbz.documents_positions dp
								where document=@id";
							break;
						default:
							throw new NotImplementedException();
					}
					using (var sqlDA = new NpgsqlDataAdapter(query, sqlConn))
					{
						sqlDA.SelectCommand.Parameters.AddWithValue("id", id);
						sqlDA.Fill(result);
					}

					/// ARTICLES
					if (module == Global.Module.ARTICLES)
                    {
						result.Columns["converter"].DefaultValue = 1.0;
						result.Columns["default"].DefaultValue = false;
					}
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania pozycji instancji", ex, module, id);
			}

			return result;
		}
		/// <summary>
		/// Pobiera z sekwencji ID nowej instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		internal static int NewInstanceID(string module)
		{
			int result = 0;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlCmd = new NpgsqlCommand($"select nextval('wbz.{module}_id_seq')", sqlConn))
				{
					result = Convert.ToInt32(sqlCmd.ExecuteScalar());
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania nowego identyfikatora", ex, module, 0);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Instancja</param>
		/// <param name="mode">Tryb</param>
		internal static bool SetInstance<T>(string module, T instance, Commands.Type mode)
		{
			bool result = false;
			string query;
			int oldstatus = 0;
			
			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					NpgsqlCommand sqlCmd;

					switch (module)
					{
						/// ARTICLES
						case Global.Module.ARTICLES:
							var article = instance as M_Article;
							query = @"insert into wbz.articles (id, codename, name, ean, archival, comment, icon)
								values (@id, @codename, @name, @ean, @archival, @comment, @icon)
								on conflict(id) do
								update set codename=@codename, name=@name, ean=@ean,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", article.ID);
								sqlCmd.Parameters.AddWithValue("codename", article.Codename);
								sqlCmd.Parameters.AddWithValue("name", article.Name);
								sqlCmd.Parameters.AddWithValue("ean", article.EAN);
								sqlCmd.Parameters.AddWithValue("archival", article.Archival);
								sqlCmd.Parameters.AddWithValue("comment", article.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)article.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, article.ID, $"{(mode==Commands.Type.EDIT ? "Edytowano" : "Utworzono")} towar: {article.Name}.", sqlTran);

							///measures
							foreach (DataRow measure in article.Measures.Rows)
							{
								///add
								if (measure.RowState == DataRowState.Added)
								{
									using (sqlCmd = new NpgsqlCommand(@"insert into wbz.articles_measures (article, name, converter, ""default"")
										values (@article, @name, @converter, @default)", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.AddWithValue("article", article.ID);
										sqlCmd.Parameters.AddWithValue("name", measure["name"]);
										sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
										sqlCmd.Parameters.AddWithValue("default", measure["default"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, module, article.ID, $"Dodano jednostkę miary {measure["name"]}.", sqlTran);
								}
								///edit
								else if (measure.RowState == DataRowState.Modified)
								{
									using (sqlCmd = new NpgsqlCommand(@"update wbz.articles_measures
										set name=@name, converter=@converter, ""default""=@default
										where id=@id", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.AddWithValue("id", measure["id", DataRowVersion.Original]);
										sqlCmd.Parameters.AddWithValue("name", measure["name"]);
										sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
										sqlCmd.Parameters.AddWithValue("default", measure["default"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, module, article.ID, $"Edytowano jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlTran);
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
									SetLog(Global.User.ID, module, article.ID, $"Usunięto jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlTran);
								}
							}
							break;
						/// ATTACHMENTS
						case Global.Module.ATTACHMENTS:
							query = @"";
							break;
						/// ATTRIBUTES_CLASSES
						case Global.Module.ATTRIBUTES_CLASSES:
							var attributeClass = instance as M_AttributeClass;
							query = @"insert into wbz.attributes_classes (id, module, name, type, required, archival, comment, icon)
								values (@id, @module, @name, @type, @required, @archival, @comment, @icon)
								on conflict(id) do
								update set module=@module, name=@name, type=@type, required=@required,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", attributeClass.ID);
								sqlCmd.Parameters.AddWithValue("module", attributeClass.Module);
								sqlCmd.Parameters.AddWithValue("name", attributeClass.Name);
								sqlCmd.Parameters.AddWithValue("type", attributeClass.Type);
								sqlCmd.Parameters.AddWithValue("required", attributeClass.Required);
								sqlCmd.Parameters.AddWithValue("archival", attributeClass.Archival);
								sqlCmd.Parameters.AddWithValue("comment", attributeClass.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)attributeClass.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, attributeClass.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} klasę atrybutu: {attributeClass.Name}.", sqlTran);

							///values
							foreach (DataRow value in attributeClass.Values.Rows)
							{
								///add
								if (value.RowState == DataRowState.Added)
								{
									using (sqlCmd = new NpgsqlCommand(@"insert into wbz.attributes_values (class, value, archival)
										values (@class, @value, @archival)", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.AddWithValue("class", attributeClass.ID);
										sqlCmd.Parameters.AddWithValue("value", value["name"]);
										sqlCmd.Parameters.AddWithValue("archival", value["archival"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, module, attributeClass.ID, $"Dodano wartość {value["name"]}.", sqlTran);
								}
								///edit
								else if (value.RowState == DataRowState.Modified)
								{
									using (sqlCmd = new NpgsqlCommand(@"update wbz.attributes_values
										set value=@value, archival=@archival
										where id=@id", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.AddWithValue("id", value["id", DataRowVersion.Original]);
										sqlCmd.Parameters.AddWithValue("value", value["name"]);
										sqlCmd.Parameters.AddWithValue("archival", value["archival"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, module, attributeClass.ID, $"Edytowano wartość {value["name", DataRowVersion.Original]}.", sqlTran);
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
									SetLog(Global.User.ID, module, attributeClass.ID, $"Usunięto wartość {value["name", DataRowVersion.Original]}.", sqlTran);
								}
							}
							break;
						/// COMPANIES
						case Global.Module.COMPANIES:
							var company = instance as M_Company;
							query = @"insert into wbz.companies (id, codename, name, branch, nip, regon, postcode, city, address, archival, comment, icon)
								values (@id, @codename, @name, @branch, @nip, @regon, @postcode, @city, @address, @archival, @comment, @icon)
								on conflict(id) do
								update set codename=@codename, name=@name, branch=@branch, nip=@nip, regon=@regon,
									postcode=@postcode, city=@city, address=@address,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", company.ID);
								sqlCmd.Parameters.AddWithValue("codename", company.Codename);
								sqlCmd.Parameters.AddWithValue("name", company.Name);
								sqlCmd.Parameters.AddWithValue("branch", company.Branch);
								sqlCmd.Parameters.AddWithValue("nip", company.NIP);
								sqlCmd.Parameters.AddWithValue("regon", company.REGON);
								sqlCmd.Parameters.AddWithValue("postcode", company.Postcode);
								sqlCmd.Parameters.AddWithValue("city", company.City);
								sqlCmd.Parameters.AddWithValue("address", company.Address);
								sqlCmd.Parameters.AddWithValue("archival", company.Archival);
								sqlCmd.Parameters.AddWithValue("comment", company.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)company.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, company.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} firmę: {company.Name}.", sqlTran);
							break;
						/// DISTRIBUTIONS
						case Global.Module.DISTRIBUTIONS:
							query = @"";
							break;
						/// DOCUMENTS
						case Global.Module.DOCUMENTS:
							var document = instance as M_Document;
							using (sqlCmd = new NpgsqlCommand(@"select status from wbz.documents where id=@id", sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", document.ID);
								oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
							}
							query = @"insert into wbz.documents (id, name, type, store, company, dateissue, status, archival, comment, icon)
								values (@id, @name, @type, @store, @company, @dateissue, @status, @archival, @comment, @icon)
								on conflict(id) do
								update set name=@name, type=@type, store=@store, company=@company, dateissue=@dateissue, status=@status,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", document.ID);
								sqlCmd.Parameters.AddWithValue("name", document.Name);
								sqlCmd.Parameters.AddWithValue("type", document.Type);
								sqlCmd.Parameters.AddWithValue("store", document.Store);
								sqlCmd.Parameters.AddWithValue("company", document.Company);
								sqlCmd.Parameters.AddWithValue("dateissue", document.DateIssue);
								sqlCmd.Parameters.AddWithValue("status", document.Status);
								sqlCmd.Parameters.AddWithValue("archival", document.Archival);
								sqlCmd.Parameters.AddWithValue("comment", document.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)document.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, document.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} dokument: {document.Name}.", sqlTran);

							///positions
							foreach (DataRow position in document.Positions.Rows)
							{
								///add
								if (position.RowState == DataRowState.Added)
								{
									using (sqlCmd = new NpgsqlCommand(@"insert into wbz.documents_positions (document, position, article, amount, cost)
										values (@document, @position, @article, (@amount * wbz.ArtDefMeaCon(@article)),
											@cost)", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.Clear();
										sqlCmd.Parameters.AddWithValue("document", document.ID);
										sqlCmd.Parameters.AddWithValue("position", position["position"]);
										sqlCmd.Parameters.AddWithValue("article", position["article"]);
										sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
										sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
										sqlCmd.Parameters.AddWithValue("cost", position["cost"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, "documents", document.ID, $"Dodano pozycję {position["position"]}.", sqlTran);
								}
								///edit
								else if (position.RowState == DataRowState.Modified)
								{
									using (sqlCmd = new NpgsqlCommand(@"update wbz.documents_positions
										set position=@position, article=@article, amount=(@amount * wbz.ArtDefMeaCon(@article)),
											cost=@cost
										where id=@id", sqlConn, sqlTran))
									{
										sqlCmd.Parameters.Clear();
										sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
										sqlCmd.Parameters.AddWithValue("position", position["position"]);
										sqlCmd.Parameters.AddWithValue("article", position["article"]);
										sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
										sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
										sqlCmd.Parameters.AddWithValue("cost", position["cost"]);
										sqlCmd.ExecuteNonQuery();
									}
									SetLog(Global.User.ID, "documents", document.ID, $"Edytowano pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
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
									SetLog(Global.User.ID, "documents", document.ID, $"Usunięto pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
								}

								///update articles amounts
								if (oldstatus != document.Status)
								{
									if (oldstatus <= 0 && document.Status > 0)
										ChangeArticleAmount(document.Store, (int)position["article"], (double)position["amount"], (string)position["measure"], false, sqlConn, sqlTran);
									else if (oldstatus > 0 && document.Status < 0)
										ChangeArticleAmount(document.Store, (int)position["article"], -(double)position["amount"], (string)position["measure"], false, sqlConn, sqlTran);
								}
							}
							break;
						/// EMPLOYEES
						case Global.Module.EMPLOYEES:
							var employee = instance as M_Employee;
							query = @"insert into wbz.employees (id, ""user"", forename, lastname, department, position,
									email, phone, city, address, postcode, archival, comment, icon)
								values (@id, @user, @forename, @lastname, @department, @position,
									@email, @phone, @city, @address, @postcode, @archival, @comment, @icon)
								on conflict(id) do
								update set ""user""=@user, forename=@forename, lastname=@lastname, department=@department, position=@position,
									email=@email, phone=@phone, city=@city, address=@address, postcode=@postcode,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", employee.ID);
								sqlCmd.Parameters.AddWithValue("user", employee.User);
								sqlCmd.Parameters.AddWithValue("forename", employee.Forename);
								sqlCmd.Parameters.AddWithValue("lastname", employee.Lastname);
								sqlCmd.Parameters.AddWithValue("department", employee.Department);
								sqlCmd.Parameters.AddWithValue("position", employee.Position);
								sqlCmd.Parameters.AddWithValue("email", employee.Email);
								sqlCmd.Parameters.AddWithValue("phone", employee.Phone);
								sqlCmd.Parameters.AddWithValue("city", employee.City);
								sqlCmd.Parameters.AddWithValue("address", employee.Address);
								sqlCmd.Parameters.AddWithValue("postcode", employee.Postcode);
								sqlCmd.Parameters.AddWithValue("archival", employee.Archival);
								sqlCmd.Parameters.AddWithValue("comment", employee.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)employee.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, employee.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} pracownika: {employee.Fullname}.", sqlTran);
							break;
						/// FAMILIES
						case Global.Module.FAMILIES:
							var family = instance as M_Family;
							query = @"insert into wbz.families (id, declarant, lastname, members, postcode, city, address,
									status, c_sms, c_call, c_email, archival, comment, icon)
								values (@id, @declarant, @lastname, @members, @postcode, @city, @address,
									@status, @c_sms, @c_call, @c_email, @archival, @comment, @icon)
								on conflict(id) do
								update set declarant=@declarant, lastname=@lastname, members=@members, postcode=@postcode, city=@city, address=@address,
									status=@status, c_sms=@c_sms, c_call=@c_call, c_email=@c_email,
									archival=@archival, comment=@comment, icon=@icon";
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
								sqlCmd.Parameters.AddWithValue("archival", family.Archival);
								sqlCmd.Parameters.AddWithValue("comment", family.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)family.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, family.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} rodzinę: {family.Lastname}.", sqlTran);
							break;
						/// GROUPS
						case Global.Module.GROUPS:
							var group = instance as M_Group;
							query = @"insert into wbz.groups (id, module, name, instance, owner, archival, comment, icon)
								values (@id, @module, @name, @instance, @owner, @archival, @comment, @icon)
								on conflict(id) do
								update set module=@module, name=@name, instance=@instance, owner=@owner, archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", group.ID);
								sqlCmd.Parameters.AddWithValue("module", group.Module);
								sqlCmd.Parameters.AddWithValue("name", group.Name);
								sqlCmd.Parameters.AddWithValue("instance", group.Instance > 0 ? (object)group.Instance : DBNull.Value);
								sqlCmd.Parameters.AddWithValue("owner", group.Owner);
								sqlCmd.Parameters.AddWithValue("archival", group.Archival);
								sqlCmd.Parameters.AddWithValue("comment", group.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)group.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, group.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} grupę: {group.Name}.", sqlTran);
							break;
						/// LOGS
						case Global.Module.LOGS:
							var log = instance as M_Log;
							query = @"insert into wbz.logs (""user"", module, instance, type, content)
								values (@user, @module, @instance, @type, @content)
								on conflict(id) do
								update set ""user""=@user, module=@module, instance=@instance, type=@type, content=@content";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("user", log.User);
								sqlCmd.Parameters.AddWithValue("module", log.Module);
								sqlCmd.Parameters.AddWithValue("instance", log.Instance);
								sqlCmd.Parameters.AddWithValue("type", log.Group);
								sqlCmd.Parameters.AddWithValue("content", log.Content);
								sqlCmd.ExecuteNonQuery();
							}
							break;
						/// STORES
						case Global.Module.STORES:
							var store = instance as M_Store;
							query = @"insert into wbz.stores (id, codename, name, city, address, postcode, archival, comment, icon)
								values (@id, @codename, @name, @city, @address, @postcode, @archival, @comment, @icon)
								on conflict(id) do
								update set codename=@codename, name=@name, city=@city, address=@address, postcode=@postcode,
									archival=@archival, comment=@comment, icon=@icon";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", store.ID);
								sqlCmd.Parameters.AddWithValue("codename", store.Codename);
								sqlCmd.Parameters.AddWithValue("name", store.Name);
								sqlCmd.Parameters.AddWithValue("city", store.City);
								sqlCmd.Parameters.AddWithValue("address", store.Address);
								sqlCmd.Parameters.AddWithValue("postcode", store.Postcode);
								sqlCmd.Parameters.AddWithValue("archival", store.Archival);
								sqlCmd.Parameters.AddWithValue("comment", store.Comment);
								sqlCmd.Parameters.AddWithValue("icon", (object)store.Icon ?? DBNull.Value);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, store.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} magazyn: {store.Name}.", sqlTran);
							break;
						/// USERS
						case Global.Module.USERS:
							var user = instance as M_User;
							query = @"insert into wbz.users (id, username, password, forename, lastname, email, phone, blocked, archival)
								values (@id, @username, @password, @forename, @lastname, @email, @phone, @blocked, @archival)
								on conflict(id) do
								update set username=@username, " + (!string.IsNullOrEmpty(user.Newpass) ? "password = @newpass," : "") + @"
									forename = @forename, lastname = @lastname, email = @email, phone = @phone, blocked = @blocked, archival = @archival";
							using (sqlCmd = new NpgsqlCommand(query, sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("id", user.ID);
								sqlCmd.Parameters.AddWithValue("username", user.Username);
								sqlCmd.Parameters.AddWithValue("password", Global.sha256(user.Newpass));
								sqlCmd.Parameters.AddWithValue("forename", user.Forename);
								sqlCmd.Parameters.AddWithValue("lastname", user.Lastname);
								sqlCmd.Parameters.AddWithValue("email", user.Email);
								sqlCmd.Parameters.AddWithValue("phone", user.Phone);
								sqlCmd.Parameters.AddWithValue("blocked", user.Blocked);
								sqlCmd.Parameters.AddWithValue("archival", user.Archival);
								sqlCmd.ExecuteNonQuery();
							}
							SetLog(Global.User.ID, module, user.ID, $"{(mode == Commands.Type.EDIT ? "Edytowano" : "Utworzono")} użytkownika: {user.Fullname}.", sqlTran);

							/// permissions
							using (sqlCmd = new NpgsqlCommand(@"delete from wbz.users_permissions where ""user""=@user", sqlConn, sqlTran))
							{
								sqlCmd.Parameters.AddWithValue("user", user.ID);
								sqlCmd.ExecuteNonQuery();
							}
							foreach (string perm in user.Perms)
							{
								using (sqlCmd = new NpgsqlCommand(@"insert into wbz.users_permissions (""user"", perm)
									values (@user, @perm)", sqlConn, sqlTran))
								{
									sqlCmd.Parameters.AddWithValue("user", user.ID);
									sqlCmd.Parameters.AddWithValue("perm", perm);
									sqlCmd.ExecuteNonQuery();
								}
							}
							break;
						default:
							throw new NotImplementedException();
					}

					sqlTran.Commit();
				}

				result = true;
			}
			catch (Exception ex)
			{
				Error("Błąd podczas zapisywania instancji", ex, module, (instance as M).ID);
			}
			
			return result;
		}
		/// <summary>
		/// Usunięcie instancji
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteInstance(string module, int id, string name)
		{
			bool result = false;
			string query;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					switch (module)
					{
						/// ARTICLES
						case Global.Module.ARTICLES:
							query = @"delete from wbz.stores_articles where article=@id;
								delete from wbz.articles where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto towar: {name}", sqlTran);
							break;
						/// ATTACHMENTS
						case Global.Module.ATTACHMENTS:
							query = @"delete from wbz.attachments where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto załącznik: {name}", sqlTran);
							break;
						/// ATTRIBUTES_CLASSES
						case Global.Module.ATTRIBUTES_CLASSES:
							query = @"delete from wbz.attributes where class=@id;
								delete from wbz.attributes_classes where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto klasę atrybutu: {name}", sqlTran);
							break;
						/// COMPANIES
						case Global.Module.COMPANIES:
							query = @"delete from wbz.companies where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto firmę: {name}", sqlTran);
							break;
						/// DISTRIBUTIONS
						case Global.Module.DISTRIBUTIONS:
							query = @"";
							break;
						/// DOCUMENTS
						case Global.Module.DOCUMENTS:
							query = @"";
							break;
						/// EMPLOYEES
						case Global.Module.EMPLOYEES:
							query = @"delete from wbz.employees where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto pracownika: {name}", sqlTran);
							break;
						/// FAMILIES
						case Global.Module.FAMILIES:
							query = @"delete from wbz.families where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto rodzinę: {name}", sqlTran);
							break;
						/// GROUPS
						case Global.Module.GROUPS:
							query = @"delete from wbz.groups where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto grupę: {name}", sqlTran);
							break;
						/// LOGS
						case Global.Module.LOGS:
							query = @"delete from wbz.logs where id=@id";
							break;
						/// STORES
						case Global.Module.STORES:
							query = @"delete from wbz.stores_articles where store=@id;
								delete from wbz.stores where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto magazyn: {name}", sqlTran);
							break;
						/// USERS
						case Global.Module.USERS:
							query = @"delete from wbz.users where id=@id";
							SetLog(Global.User.ID, module, id, $"Usunięto użytkownika: {name}", sqlTran);
							break;
						default:
							throw new NotImplementedException();
					}

					using (var sqlCmd = new NpgsqlCommand(query, sqlConn))
					{
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();
					}
					ClearObject(module, id, sqlConn, sqlTran);

					sqlTran.Commit();
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas usuwania instancji", ex, module, id);
			}
			
			return result;
		}
#endregion

		#region additional
		/// <summary>
		/// Pobiera wartość podanej właściwości z bazy danych
		/// </summary>
		/// <param name="property">Nazwa właściwości z tabeli wbz.config</param>
		internal static string GetPropertyValue(string property)
		{
			string result = null;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlCmd = new NpgsqlCommand(@"select coalesce((select value from wbz.config where property=@property),'')", sqlConn))
				{
					sqlCmd.Parameters.AddWithValue("property", property);
					result = sqlCmd.ExecuteScalar().ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
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
			bool result = false;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlCmd = new NpgsqlCommand(@"update wbz.config set value=@value where property=@property", sqlConn))
				{
					sqlCmd.Parameters.AddWithValue("property", property);
					sqlCmd.Parameters.AddWithValue("value", value);
					sqlCmd.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Usuwa wszystkie kontakty, atrybuty i załączniki przypisane do obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="id">ID instancji</param>
		internal static bool ClearObject(string module, int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					ClearObject(module, id, sqlConn, sqlTran);
					sqlTran.Commit();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Usuwa wszystkie kontakty, atrybuty i załączniki przypisane do obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="id">ID instancji</param>
		internal static bool ClearObject(string module, int id, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
		{
			bool result = false;

			try
			{
				using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.attachments where module=@module and instance=@instance", sqlConn, sqlTran))
				{
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", id);
					sqlCmd.ExecuteNonQuery();
				}
				using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes
					where class in (select id from wbz.attributes_classes where module=@module) and instance=@instance", sqlConn, sqlTran))
				{
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", id);
					sqlCmd.ExecuteNonQuery();
				}
				using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.contacts where module=@module and instance=@instance", sqlConn, sqlTran))
				{
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", id);
					sqlCmd.ExecuteNonQuery();
				}
				using (var sqlCmd = new NpgsqlCommand(@"delete from wbz.groups where module=@module and instance=@instance", sqlConn, sqlTran))
				{
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", id);
					sqlCmd.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Pobiera zawartość załącznika o podanych parametrach
		/// </summary>
		/// <param name="id">ID załącznika</param>
		internal static byte[] GetAttachmentFile(int id)
		{
			byte[] result = null;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select file
						from wbz.attachments
						where id=@id", sqlConn);
					sqlCmd.Parameters.AddWithValue("id", id);
					result = (byte[])sqlCmd.ExecuteScalar();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Zapisuje załącznik o podanych parametrach
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="name">Nazwa załącznika</param>
		/// <param name="file">Plik</param>
		internal static int SetAttachment(string module, int instance, string name, byte[] file, string comment)
		{
			int result = 0;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"insert into wbz.attachments (""user"", module, instance, name, file, comment)
						values (@user, @module, @instance, @name, @file, @comment) returning id", sqlConn);
					sqlCmd.Parameters.AddWithValue("user", Global.User.ID);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.Parameters.AddWithValue("name", name);
					sqlCmd.Parameters.AddWithValue("file", file);
					sqlCmd.Parameters.AddWithValue("comment", comment);
					result = Convert.ToInt32(sqlCmd.ExecuteScalar());

					SetLog(Global.User.ID, module, instance, $"Dodano załącznik {name}.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Pobiera atrybuty dla podanego modułu i obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="filter">Filtr SQL</param>
		internal static List<M_Attribute> ListAttributes(string module, int instance, string filter = null)
		{
			var result = new List<M_Attribute>();

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select a.id as id, ac.module, @instance as instance,
							ac.id as class, ac.name, ac.type, ac.required, ac.icon, a.value as value
						from wbz.attributes_classes ac
						left join wbz.attributes a
							on ac.id=a.class and a.instance=@instance
						where ac.module=@module and @filter
						order by ac.name", sqlConn);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);

						foreach (DataRow row in dt.Rows)
						{
							M_Attribute c = new M_Attribute()
							{
								ID = !Convert.IsDBNull(row["id"]) ? (long)row["id"] : 0,
								Class = new M_AttributeClass()
								{
									ID = !Convert.IsDBNull(row["class"]) ? (int)row["class"] : 0,
									Module = !Convert.IsDBNull(row["module"]) ? (string)row["module"] : string.Empty,
									Name = !Convert.IsDBNull(row["name"]) ? (string)row["name"] : string.Empty,
									Type = !Convert.IsDBNull(row["type"]) ? (string)row["type"] : string.Empty,
									Required = !Convert.IsDBNull(row["required"]) ? (bool)row["required"] : false,
									Icon = !Convert.IsDBNull(row["icon"]) ? (byte[])row["icon"] : null,
									Values = GetInstancePositions(Global.Module.ATTRIBUTES_CLASSES, !Convert.IsDBNull(row["class"]) ? (int)row["class"] : 0)
								},
								Instance = !Convert.IsDBNull(row["instance"]) ? (int)row["instance"] : 0,
								Value = !Convert.IsDBNull(row["value"]) ? (string)row["value"] : string.Empty
							};
							result.Add(c);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Error("Błąd podczas pobierania listy atrybutów", ex, Global.Module.ATTRIBUTES, 0);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o atrybucie
		/// </summary>
		/// <param name="attribute">Klasa atrybutu</param>
		internal static bool UpdateAttribute(M_Attribute attribute)
		{
			bool result = false;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					///add
					if (attribute.ID == 0)
					{
						var sqlCmd = new NpgsqlCommand(@"insert into wbz.attributes (instance, class, value)
							values (@instance, @class, @value)", sqlConn);
						sqlCmd.Parameters.AddWithValue("instance", attribute.Instance);
						sqlCmd.Parameters.AddWithValue("class", attribute.Class.ID);
						sqlCmd.Parameters.AddWithValue("value", attribute.Value);
						sqlCmd.ExecuteNonQuery();
						SetLog(Global.User.ID, attribute.Class.Module, attribute.Instance, $"Dodano atrybut {attribute.Class.Name}.");
					}
					///edit
					else if (!string.IsNullOrEmpty(attribute.Value))
					{
						var sqlCmd = new NpgsqlCommand(@"update wbz.attributes
							set instance=@instance, class=@class, value=@value
							where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", attribute.ID);
						sqlCmd.Parameters.AddWithValue("instance", attribute.Instance);
						sqlCmd.Parameters.AddWithValue("class", attribute.Class.ID);
						sqlCmd.Parameters.AddWithValue("value", attribute.Value);
						sqlCmd.ExecuteNonQuery();
						SetLog(Global.User.ID, attribute.Class.Module, attribute.Instance, $"Edytowano atrybut {attribute.Class.Name}.");
					}
					///delete
					else
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes
							where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", attribute.ID);
						sqlCmd.ExecuteNonQuery();
						SetLog(Global.User.ID, attribute.Class.Module, attribute.Instance, $"Usunięto atrybut {attribute.Class.Name}.");
					}
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Pobiera kontakty dla podanego modułu i obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="filter">Filtr SQL</param>
		internal static DataTable ListContacts(string module, int instance, string filter = null)
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = connOpenedWBZ)
				{
					var sqlCmd = new NpgsqlCommand(@"select c.id, c.module, c.instance, c.email, c.phone, c.forename, c.lastname, c.""default"", c.archival
						from wbz.contacts c
						where c.module=@module and c.instance=@instance and @filter", sqlConn);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
						result.Columns["default"].DefaultValue = false;
						result.Columns["archival"].DefaultValue = false;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o kontaktach
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="contacts">Tabela kontaktów</param>
		internal static bool UpdateContacts(string module, int instance, DataTable contacts)
		{
			bool result = false;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlTran = sqlConn.BeginTransaction())
				{
					NpgsqlCommand sqlCmd;

					///contacts
					foreach (DataRow contact in contacts.Rows)
					{
						///add
						if (contact.RowState == DataRowState.Added)
						{
							sqlCmd = new NpgsqlCommand(@"insert into wbz.contacts (module, instance, email, phone, forename, lastname, ""default"", archival)
								values (@module, @instance, @email, @phone, @forename, @lastname, @default, @archival)", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("module", module);
							sqlCmd.Parameters.AddWithValue("instance", instance);
							sqlCmd.Parameters.AddWithValue("email", contact["email"]);
							sqlCmd.Parameters.AddWithValue("phone", contact["phone"]);
							sqlCmd.Parameters.AddWithValue("forename", contact["forename"]);
							sqlCmd.Parameters.AddWithValue("lastname", contact["lastname"]);
							sqlCmd.Parameters.AddWithValue("default", contact["default"]);
							sqlCmd.Parameters.AddWithValue("archival", contact["archival"]);
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, module, instance, $"Dodano kontakt {contact["forename"]} {contact["lastname"]}.", sqlTran);
						}
						///edit
						else if (contact.RowState == DataRowState.Modified)
						{
							sqlCmd = new NpgsqlCommand(@"update wbz.contacts
								set email=@email, phone=@phone, forename=@forename, lastname=@lastname, ""default""=@default, archival=@archival
								where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", contact["id", DataRowVersion.Original]);
							sqlCmd.Parameters.AddWithValue("email", contact["email"]);
							sqlCmd.Parameters.AddWithValue("phone", contact["phone"]);
							sqlCmd.Parameters.AddWithValue("forename", contact["forename"]);
							sqlCmd.Parameters.AddWithValue("lastname", contact["lastname"]);
							sqlCmd.Parameters.AddWithValue("default", contact["default"]);
							sqlCmd.Parameters.AddWithValue("archival", contact["archival"]);
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, module, instance, $"Edytowano kontakt {contact["forename", DataRowVersion.Original]} {contact["lastname", DataRowVersion.Original]}.", sqlTran);
						}
						///delete
						else if (contact.RowState == DataRowState.Deleted)
						{
							sqlCmd = new NpgsqlCommand(@"delete from wbz.contacts
								where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", contact["id", DataRowVersion.Original]);
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, module, instance, $"Usunięto kontakt {contact["forename", DataRowVersion.Original]} {contact["lastname", DataRowVersion.Original]}.", sqlTran);
						}
					}

					sqlTran.Commit();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Zapisuje log o podanych parametrach
		/// </summary>
		/// <param name="user">Numer ID użytkownika</param>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="content">Treść logu</param>
		internal static bool SetLog(int user, string module, int instance, string content, NpgsqlTransaction sqlTran = null)
		{
			bool result = false;

			if (M_Config.Logs_Enabled != "1")
				return true;

			try
			{
				using (var sqlConn = connOpenedWBZ)
				using (var sqlCmd = new NpgsqlCommand(@"insert into wbz.logs (""user"", module, instance, content)
					values (@user, @module, @instance, @content)", sqlConn, sqlTran))
				{
					sqlCmd.Parameters.AddWithValue("user", user);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.Parameters.AddWithValue("content", content);
					sqlCmd.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception ex)
			{
				Error("Błąd podczas zapisywania logu", ex, Global.Module.LOGS, 0, true, false);
			}

			return result;
		}
		#endregion
	}
}
