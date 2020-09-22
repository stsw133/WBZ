using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ
{
	internal static class SQL
	{
		internal static string connWBZ = null; ///przypisywanie połączenia w oknie logowania

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

		#region mainDB
		/// <summary>
		/// Pobiera wartość podanej właściwości z bazy danych
		/// </summary>
		/// <param name="property">Nazwa właściwości z tabeli wbz.config</param>
		internal static string GetPropertyValue(string property)
		{
			string result = null;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select coalesce((select value from wbz.config where property=@property),'')", sqlConn);
					sqlCmd.Parameters.AddWithValue("property", property);
					result = sqlCmd.ExecuteScalar().ToString();

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"update wbz.config set value=@value where property=@property", sqlConn);
					sqlCmd.Parameters.AddWithValue("property", property);
					sqlCmd.Parameters.AddWithValue("value", value);
					sqlCmd.ExecuteNonQuery();

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera logi dla podanego modułu, obiektu i użytkownika
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="user">Numer ID użytkownika</param>
		internal static List<C_Log> ListLogs(string module, int? instance, int? user)
		{
			var result = new List<C_Log>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select l.id, l.""user"", u.lastname || ' ' || u.forename as userfullname,
							l.module, l.instance, l.content, l.datetime
						from wbz.logs l
						left join wbz.users u
							on l.""user"" = u.id
						where l.module like ('%' || @module || '%')"
						+ (instance != null ? $" and l.instance={instance}" : "")
						+ (user != null ? $" and l.\"user\"={user}" : "")
						+ " order by l.datetime desc", sqlConn);
					sqlCmd.Parameters.AddWithValue("module", module);
					//sqlCmd.Parameters.AddWithValue("instance", instance);
					//sqlCmd.Parameters.AddWithValue("user", user);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Log>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera listę logów
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Log> ListLogs(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "datetime", bool desc = true)
		{
			var result = new List<C_Log>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select l.id, l.""user"", u.lastname || ' ' || u.forename as userfullname,
							l.module, l.instance, l.content, l.datetime
						from wbz.logs l
						left join wbz.users u
							on l.""user"" = u.id
						where @filter
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? "1=1");
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Log>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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

			if (C_Config.Logs_Enabled != "1")
				return true;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"insert into wbz.logs (""user"", module, instance, content)
						values (@user, @module, @instance, @content)", sqlConn, sqlTran);
					sqlCmd.Parameters.AddWithValue("user", user);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.Parameters.AddWithValue("content", content);
					sqlCmd.ExecuteNonQuery();

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usuwa log o podanych parametrach
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteLog(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"delete from wbz.logs where id=@id", sqlConn);
					sqlCmd.Parameters.AddWithValue("id", id);
					sqlCmd.ExecuteNonQuery();

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera atrybuty dla podanego modułu i obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="filter">Filtr SQL</param>
		internal static List<C_Attribute> ListAttributes(string module, int instance, string filter = null)
		{
			var result = new List<C_Attribute>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select a.id as id, ac.module, @instance as instance,
							ac.id as class, ac.name, ac.type, ac.values, ac.required,
							a.value as value
						from wbz.attributes_classes ac
						left join wbz.attributes a
							on ac.id=a.class and a.instance=@instance
						where ac.module=@module and @filter", sqlConn);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);

						foreach (DataRow row in dt.Rows)
						{
							C_Attribute c = new C_Attribute()
							{
								ID = !Convert.IsDBNull(row["id"]) ? (long)row["id"] : 0,
								Class = new C_AttributeClass()
								{
									ID = !Convert.IsDBNull(row["class"]) ? (int)row["class"] : 0,
									Module = !Convert.IsDBNull(row["module"]) ? (string)row["module"] : "",
									Name = !Convert.IsDBNull(row["name"]) ? (string)row["name"] : "",
									Type = !Convert.IsDBNull(row["type"]) ? (string)row["type"] : "",
									Values = !Convert.IsDBNull(row["values"]) ? (string)row["values"] : "",
									Required = !Convert.IsDBNull(row["required"]) ? (bool)row["required"] : false
								},
								Instance = !Convert.IsDBNull(row["instance"]) ? (int)row["instance"] : 0,
								Value = !Convert.IsDBNull(row["value"]) ? (string)row["value"] : ""
							};
							result.Add(c);
						}
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o atrybucie
		/// </summary>
		/// <param name="attribute">Klasa atrybutu</param>
		internal static bool UpdateAttribute(C_Attribute attribute)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera załączniki dla podanego modułu i obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		internal static List<C_Attachment> ListAttachments(string module, int instance)
		{
			var result = new List<C_Attachment>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select a.id, a.""user"", a.module, a.instance, a.name, null as file
						from wbz.attachments a
						where a.module=@module and a.instance=@instance", sqlConn);
					sqlCmd.Parameters.AddWithValue("module", module);
					sqlCmd.Parameters.AddWithValue("instance", instance);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Attachment>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select file
						from wbz.attachments
						where id=@id", sqlConn);
					sqlCmd.Parameters.AddWithValue("id", id);
					result = (byte[])sqlCmd.ExecuteScalar();

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Zapisuje załącznik o podanych parametrach
		/// </summary>
		/// <param name="user">Numer ID użytkownika</param>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		/// <param name="name">Nazwa załącznika</param>
		/// <param name="file">Plik</param>
		internal static int SetAttachment(string module, int instance, string name, byte[] file, string comment)
		{
			int result = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usuwa załącznik o podanych parametrach
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteAttachment(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"delete from wbz.attachments where id=@id", sqlConn);
					sqlCmd.Parameters.AddWithValue("id", id);
					sqlCmd.ExecuteNonQuery();

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usuwa wszystkie kontakty, atrybuty i załączniki przypisane do obiektu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="instance">Numer ID obiektu</param>
		internal static bool ClearObject(string module, int instance, NpgsqlConnection sqlConn, NpgsqlTransaction sqlTran)
		{
			bool result = false;

			try
			{
				var sqlCmd = new NpgsqlCommand(@"delete from wbz.contacts where module=@module and instance=@instance", sqlConn, sqlTran);
				sqlCmd.Parameters.AddWithValue("module", module);
				sqlCmd.Parameters.AddWithValue("instance", instance);
				sqlCmd.ExecuteNonQuery();

				sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes
					where class in (select id from wbz.attributes_classes where module=@module)
						and instance=@instance", sqlConn, sqlTran);
				sqlCmd.Parameters.AddWithValue("module", module);
				sqlCmd.Parameters.AddWithValue("instance", instance);
				sqlCmd.ExecuteNonQuery();

				sqlCmd = new NpgsqlCommand(@"delete from wbz.attachments where module=@module and instance=@instance", sqlConn, sqlTran);
				sqlCmd.Parameters.AddWithValue("module", module);
				sqlCmd.Parameters.AddWithValue("instance", instance);
				sqlCmd.ExecuteNonQuery();

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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
						Global.User = user.DataTableToList<C_User>()[0];
						if (Global.User.Blocked)
							MessageBox.Show("Użytkownik o podanym loginie jest zablokowany.");
						else
						{
							sqlCmd = new NpgsqlCommand(@"select id, user, perm
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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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
									(@id, 'admin_users_preview'),
									(@id, 'admin_users_save'),
									(@id, 'admin_users_delete')", sqlConn, sqlTran);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						sqlTran.Commit();
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		/// <summary>
		/// Pobiera nazwiska i imiona użytkowników (przydatne do bindowania comboboxów gdy szukamy twórców lub modyfikatorów)
		/// </summary>
		internal static DataTable GetUsersFullnames()
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select id, lastname || ' ' || forename as fullname
						from wbz.users
						order by lastname asc, forename asc", sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}

		/// <summary>
		/// Pobiera nazwy magazynów (przydatne do bindowania comboboxów)
		/// </summary>
		internal static DataTable GetStoresNames()
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select id, coalesce(nullif(codename,''), name) as name
						from wbz.stores
						order by codename asc, name asc", sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}

		#region module "Admin - Users"
		/// <summary>
		/// Pobiera listę użytkowników
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_User> ListUsers(string filter = null, int page = 0)
		{
			var result = new List<C_User>();

			try
			{
				int limit = Properties.Settings.Default.config_UsersList_LimitPerPage;
				int offset = page * limit;
				string order = $"{Properties.Settings.Default.config_UsersList_Sort1By} {(Properties.Settings.Default.config_UsersList_Sort1Order ? "desc" : "asc")}";
				if (!string.IsNullOrEmpty(Properties.Settings.Default.config_UsersList_Sort2By))
					order += $", {Properties.Settings.Default.config_UsersList_Sort2By} {(Properties.Settings.Default.config_UsersList_Sort2Order ? "desc" : "asc")}";

				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select u.id, u.username, '' as newpass, u.forename, u.lastname,
							u.email, u.phone, u.blocked, u.archival
						from wbz.users u
						where @filter
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order);
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_User>();
					}

					foreach (C_User user in result)
					{
						sqlCmd = new NpgsqlCommand(@"select up.perm
							from wbz.users_permissions up
							where ""user""=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", user.ID);
						using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
						{
							var dt = new DataTable();
							sqlDA.Fill(dt);
							foreach (DataRow row in dt.Rows)
								user.Perms.Add(row["perm"].ToString());
						}
					}
					
					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o użytkowniku
		/// </summary>
		/// <param name="store">Klasa użytkownika</param>
		internal static bool SetUser(C_User user)
		{
			bool result = false;
			int ID = user.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.users wbz.users (username, password, forename, lastname, email, phone, blocked, archival)
								values (@username, @password, @forename, @lastname, @email, @phone, @blocked, @archival) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.users
								set username=@username, forename=@forename, lastname=@lastname, email=@email, phone=@phone, blocked=@blocked, archival=@archival
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("username", user.Username);
						sqlCmd.Parameters.AddWithValue("password", Global.sha256(user.Newpass));
						sqlCmd.Parameters.AddWithValue("forename", user.Forename);
						sqlCmd.Parameters.AddWithValue("lastname", user.Lastname);
						sqlCmd.Parameters.AddWithValue("email", user.Email);
						sqlCmd.Parameters.AddWithValue("phone", user.Phone);
						sqlCmd.Parameters.AddWithValue("blocked", user.Blocked);
						sqlCmd.Parameters.AddWithValue("archival", user.Archival);

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "users", ID, $"Utworzono użytkownika {user.Forename} {user.Lastname}", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							///set password if given
							if (!string.IsNullOrEmpty(user.Newpass))
							{
								sqlCmd = new NpgsqlCommand(@"update wbz.users
									set password=@newpass
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.AddWithValue("id", ID);
								sqlCmd.Parameters.AddWithValue("newpass", Global.sha256(user.Newpass));
								sqlCmd.ExecuteNonQuery();
							}

							SetLog(Global.User.ID, "users", ID, $"Edytowano użytkownika {user.Forename} {user.Lastname}", sqlTran);
						}

						///permissions
						sqlCmd = new NpgsqlCommand(@"delete from wbz.users_permissions where ""user""=@user", sqlConn, sqlTran);
						sqlCmd.Parameters.AddWithValue("user", ID);
						sqlCmd.ExecuteNonQuery();

						foreach (string perm in user.Perms)
						{
							sqlCmd = new NpgsqlCommand(@"insert into wbz.users_permissions (""user"", perm)
								values (@user, @perm)", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("user", ID);
							sqlCmd.Parameters.AddWithValue("perm", perm);
							sqlCmd.ExecuteNonQuery();
						}

						sqlTran.Commit();
						user.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie użytkownika
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteUser(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.users where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("users", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "users", id, $"Usunięto użytkownika.");

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Admin - Employees"
		/// <summary>
		/// Pobiera listę pracowników
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Employee> ListEmployees(string filter = null, int page = 0)
		{
			var result = new List<C_Employee>();

			try
			{
				int limit = Properties.Settings.Default.config_EmployeesList_LimitPerPage;
				int offset = page * limit;
				string order = $"{Properties.Settings.Default.config_EmployeesList_Sort1By} {(Properties.Settings.Default.config_EmployeesList_Sort1Order ? "desc" : "asc")}";
				if (!string.IsNullOrEmpty(Properties.Settings.Default.config_EmployeesList_Sort2By))
					order += $", {Properties.Settings.Default.config_EmployeesList_Sort2By} {(Properties.Settings.Default.config_EmployeesList_Sort2Order ? "desc" : "asc")}";

				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select e.id, e.""user"", e.forename, e.lastname,
							e.department, e.position, e.email, e.phone, e.postcode, e.city, e.address, e.archival
						from wbz.employees e
						where @filter
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order);
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Employee>();

						foreach (C_Employee employee in result)
							employee.User = GetInstance("users", (int)dt.Rows.Find($"id = {employee.ID}")["user"]) as C_User;
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o pracowniku
		/// </summary>
		/// <param name="store">Klasa pracownika</param>
		internal static bool SetEmployee(C_Employee employee)
		{
			bool result = false;
			int ID = employee.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (employee.ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.employees (""user"", forename, lastname, department, position,
									email, phone, city, address, postcode, archival, comment)
								values (@user, @forename, @lastname, @department, @position,
									@email, @phone, @city, @address, @postcode, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.employees
								set ""user""=@user, forename=@forename, lastname=@lastname, department=@department, position=@position,
									email=@email, phone=@phone, city=@city, address=@address, postcode=@postcode, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("user", employee.User.ID);
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

						///add
						if (employee.ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "employees", ID, $"Utworzono pracownika.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "employees", ID, $"Edytowano pracownika.", sqlTran);
						}

						sqlTran.Commit();
						employee.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie pracownika
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteEmployee(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.employees where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("employees", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "employees", id, $"Usunięto pracownika.");

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Documents"
		/// <summary>
		/// Pobiera listę dokumentów
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Document> ListDocuments(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "dateissue", bool desc = true)
		{
			var result = new List<C_Document>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select d.id, d.name, d.store, s.name as storename, d.company, c.name as companyname,
							d.type, d.dateissue, d.status, count(dp.*) as positionscount, sum(dp.amount) as weight, sum(dp.cost) as cost,
							d.archival, d.comment
						from wbz.documents d
						left join wbz.documents_positions dp
							on dp.document=d.id
						left join wbz.companies c
							on c.id=d.company
						left join wbz.stores s
							on s.id=d.store
						where @filter
						group by d.id, c.id, s.id
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Document>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera dane o pozycjach dokumentu
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static DataTable GetDocumentPositions(int id)
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					NpgsqlCommand sqlCmd;

					///add
					if (id == 0)
					{
						sqlCmd = new NpgsqlCommand(@"select 0 as id, 0 as position, 0 as article, '' as articlename,
							0.0 as amount, '' as measure, 0.0 as cost", sqlConn);
					}
					///edit
					else
					{
						sqlCmd = new NpgsqlCommand(@"select id, position, article, (select name from wbz.articles where id=dp.article) as articlename,
								amount / wbz.ArtDefMeaCon(dp.article) as amount, coalesce(nullif(wbz.ArtDefMeaNam(dp.article),''), 'kg') as measure, cost
							from wbz.documents_positions dp
							where document=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
					}
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
						if (id == 0)
							result.Rows.Clear();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o dokumencie
		/// </summary>
		/// <param name="document">Klasa dokumentu</param>
		internal static bool SetDocument(C_Document document)
		{
			bool result = false;
			int ID = document.ID;
			int oldstatus = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///take oldstatus if invoice exists
						if (ID != 0)
						{
							sqlCmd = new NpgsqlCommand(@"select status from wbz.documents where id=@id", sqlConn, sqlTran);
							sqlCmd.Parameters.AddWithValue("id", ID);
							oldstatus = Convert.ToInt32(sqlCmd.ExecuteScalar());
						}

						///add
						if (ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.documents (name, type, store, company, dateissue, status, archival, comment)
								values (@name, @type, @store, @company, @dateissue, @status, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.documents
								set name=@name, type=@type, store=@store, company=@company,
									dateissue=@dateissue, status=@status, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("name", document.Name);
						sqlCmd.Parameters.AddWithValue("type", document.Type);
						sqlCmd.Parameters.AddWithValue("store", document.Store);
						sqlCmd.Parameters.AddWithValue("company", document.Company);
						sqlCmd.Parameters.AddWithValue("dateissue", document.DateIssue);
						sqlCmd.Parameters.AddWithValue("status", document.Status);
						sqlCmd.Parameters.AddWithValue("archival", document.Archival);
						sqlCmd.Parameters.AddWithValue("comment", document.Comment);

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "documents", ID, $"Utworzono dokument.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "documents", ID, $"Edytowano dokument.", sqlTran);
						}

						///positions
						foreach (DataRow position in document.Positions.Rows)
						{
							///add
							if (position.RowState == DataRowState.Added)
							{
								sqlCmd = new NpgsqlCommand(@"insert into wbz.documents_positions (document, position, article, amount, cost)
									values (@document, @position, @article, (@amount * wbz.ArtDefMeaCon(@article)),
										@cost)", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("document", ID);
								sqlCmd.Parameters.AddWithValue("position", position["position"]);
								sqlCmd.Parameters.AddWithValue("article", position["article"]);
								sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
								sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
								sqlCmd.Parameters.AddWithValue("cost", position["cost"]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "documents", ID, $"Dodano pozycję {position["position"]}.", sqlTran);
							}
							///edit
							else if (position.RowState == DataRowState.Modified)
							{
								sqlCmd = new NpgsqlCommand(@"update wbz.documents_positions
									set position=@position, article=@article, amount=(@amount * wbz.ArtDefMeaCon(@article)),
										cost=@cost
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
								sqlCmd.Parameters.AddWithValue("position", position["position"]);
								sqlCmd.Parameters.AddWithValue("article", position["article"]);
								sqlCmd.Parameters.AddWithValue("amount", position["amount"]);
								sqlCmd.Parameters.AddWithValue("measure", position["measure"]);
								sqlCmd.Parameters.AddWithValue("cost", position["cost"]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "documents", ID, $"Edytowano pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
							}
							///delete
							else if (position.RowState == DataRowState.Deleted)
							{
								sqlCmd = new NpgsqlCommand(@"delete from wbz.documents_positions
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.Clear();
								sqlCmd.Parameters.AddWithValue("id", position["id", DataRowVersion.Original]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "documents", ID, $"Usunięto pozycję {position["position", DataRowVersion.Original]}.", sqlTran);
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

						sqlTran.Commit();
						document.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
							var document = GetInstance("documents", id) as C_Document;
							var positions = GetDocumentPositions(id);
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
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Stores"
		/// <summary>
		/// Pobiera listę magazynów
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Store> ListStores(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "name", bool desc = false)
		{
			var result = new List<C_Store>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select s.id, s.codename, s.name, s.postcode, s.city, s.address,
							coalesce(sum(amount),0) as amount, coalesce(sum(reserved),0) as reserved,
							s.archival, s.comment
						from wbz.stores s
						left join wbz.stores_articles sa
							on s.id = sa.store
						where @filter
						group by s.id
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Store>();
					}
					
					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o magazynie
		/// </summary>
		/// <param name="store">Klasa magazynu</param>
		internal static bool SetStore(C_Store store)
		{
			bool result = false;
			int ID = store.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (store.ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.stores (codename, name, city, address, postcode, archival, comment)
								values (@codename, @name, @city, @address, @postcode, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.stores
								set codename=@codename, name=@name, city=@city, address=@address, postcode=@postcode, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("codename", store.Codename);
						sqlCmd.Parameters.AddWithValue("name", store.Name);
						sqlCmd.Parameters.AddWithValue("city", store.City);
						sqlCmd.Parameters.AddWithValue("address", store.Address);
						sqlCmd.Parameters.AddWithValue("postcode", store.Postcode);
						sqlCmd.Parameters.AddWithValue("archival", store.Archival);
						sqlCmd.Parameters.AddWithValue("comment", store.Comment);

						///add
						if (store.ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "stores", ID, $"Utworzono magazyn.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "stores", ID, $"Edytowano magazyn.", sqlTran);
						}

						sqlTran.Commit();
						store.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie magazynu
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteStore(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.stores_articles where store=@id;
							delete from wbz.stores where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("stores", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "stores", id, $"Usunięto magazyn.", sqlTran);

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Articles"
		/// <summary>
		/// Pobiera listę towarów
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		/// <param name="storeID">ID magazynu</param>
		internal static List<C_Article> ListArticles(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "name", bool desc = false, int storeID = 0)
		{
			var result = new List<C_Article>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select a.id, a.codename, a.name, a.ean, coalesce(nullif(wbz.ArtDefMeaNam(a.id),''), 'kg') as measure,
							coalesce(sum(sa.amount), 0) as amountraw, coalesce(sum(sa.amount) / wbz.ArtDefMeaCon(a.id), 0) as amount,
							coalesce(sum(sa.reserved), 0) as reservedraw, coalesce(sum(sa.reserved) / wbz.ArtDefMeaCon(a.id), 0) as reserved,
							a.archival, a.comment
						from wbz.articles a
						left join wbz.stores_articles sa
							on a.id = sa.article " + (storeID > 0 ? $"and sa.store={storeID}" : "") + @"
						where @filter
						group by a.id
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Article>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera dane o jednostkach miar towaru
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static DataTable GetArticleMeasures(int id)
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					NpgsqlCommand sqlCmd;

					///add
					if (id == 0)
					{
						sqlCmd = new NpgsqlCommand(@"select 0 as id, 'szt' as name, 1.00 as converter, false as ""default"",
								0.0 as amount, 0.0 as reserved where false", sqlConn);
					}
					///edit
					else
					{
						sqlCmd = new NpgsqlCommand(@"select am.id, am.name, am.converter, am.""default"",
								sa.amount / coalesce(nullif(am.converter,0),1) as amount, sa.reserved / coalesce(nullif(am.converter,0),1) as reserved
							from wbz.articles a
							inner join wbz.articles_measures am
								on a.id = am.article
							left join wbz.stores_articles sa
								on a.id = sa.article
							where a.id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
					}
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
						result.Columns["default"].DefaultValue = false;
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o towarze
		/// </summary>
		/// <param name="article">Klasa towaru</param>
		internal static bool SetArticle(C_Article article)
		{
			bool result = false;
			int ID = article.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (article.ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.articles (codename, name, ean, archival, comment)
								values (@codename, @name, @ean, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.articles
								set codename=@codename, name=@name, ean=@ean, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("codename", article.Codename);
						sqlCmd.Parameters.AddWithValue("name", article.Name);
						sqlCmd.Parameters.AddWithValue("ean", article.EAN);
						sqlCmd.Parameters.AddWithValue("archival", article.Archival);
						sqlCmd.Parameters.AddWithValue("comment", article.Comment);

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "articles", ID, $"Utworzono towar.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "articles", ID, $"Edytowano towar.", sqlTran);
						}

						///measures
						foreach (DataRow measure in article.Measures.Rows)
						{
							///add
							if (measure.RowState == DataRowState.Added)
							{
								sqlCmd = new NpgsqlCommand(@"insert into wbz.articles_measures (article, name, converter, ""default"")
									values (@article, @name, @converter, @default)", sqlConn, sqlTran);
								sqlCmd.Parameters.AddWithValue("article", ID);
								sqlCmd.Parameters.AddWithValue("name", measure["name"]);
								sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
								sqlCmd.Parameters.AddWithValue("default", measure["default"]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "articles", ID, $"Dodano jednostkę miary {measure["name"]}.", sqlTran);
							}
							///edit
							else if (measure.RowState == DataRowState.Modified)
							{
								sqlCmd = new NpgsqlCommand(@"update wbz.articles_measures
									set name=@name, converter=@converter, ""default""=@default
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.AddWithValue("id", measure["id", DataRowVersion.Original]);
								sqlCmd.Parameters.AddWithValue("name", measure["name"]);
								sqlCmd.Parameters.AddWithValue("converter", measure["converter"]);
								sqlCmd.Parameters.AddWithValue("default", measure["default"]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "articles", ID, $"Edytowano jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlTran);
							}
							///delete
							else if (measure.RowState == DataRowState.Deleted)
							{
								sqlCmd = new NpgsqlCommand(@"delete from wbz.articles_measures
									where id=@id", sqlConn, sqlTran);
								sqlCmd.Parameters.AddWithValue("id", measure["id", DataRowVersion.Original]);
								sqlCmd.ExecuteNonQuery();
								SetLog(Global.User.ID, "articles", ID, $"Usunięto jednostkę miary {measure["name", DataRowVersion.Original]}.", sqlTran);
							}
						}

						sqlTran.Commit();
						article.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie towaru
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteArticle(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.stores_articles where article=@id;
							delete from wbz.articles where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("articles", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "articles", id, $"Usunięto towar.");

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select wbz.artdefmeacon(@article)", sqlConn);
					sqlCmd.Parameters.AddWithValue("article", article);
					result = Convert.ToDouble(sqlCmd.ExecuteScalar());

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Companies"
		/// <summary>
		/// Pobiera listę firm
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Company> ListCompanies(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "name", bool desc = false)
		{
			var result = new List<C_Company>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select c.id, c.codename, c.name, c.branch, c.nip, c.regon, c.postcode, c.city, c.address,
							c.archival, c.comment
						from wbz.companies c
						where @filter
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Company>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o firmie
		/// </summary>
		/// <param name="company">Klasa firmy</param>
		internal static bool SetCompany(C_Company company)
		{
			bool result = false;
			int ID = company.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.companies (codename, name, branch, nip, regon, postcode, city, address, archival, comment)
								values (@codename, @name, @branch, @nip, @regon, @postcode, @city, @address, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.companies
								set codename=@codename, name=@name, branch=@branch, nip=@nip, regon=@regon,
									postcode=@postcode, city=@city, address=@address, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
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

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "companies", ID, $"Utworzono firmę.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "companies", ID, $"Edytowano firmę.", sqlTran);
						}

						sqlTran.Commit();
						company.ID = ID;
					}

					sqlConn.Close();
				}

				if (UpdateContacts("companies", company.ID, company.Contacts))
					result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie firmy
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteCompany(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.companies where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("companies", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "companies", id, $"Usunięto firmę.");

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Families"
		/// <summary>
		/// Pobiera listę rodzin
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Family> ListFamilies(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "lastname", bool desc = false)
		{
			var result = new List<C_Family>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select f.id, f.declarant, f.lastname, f.members, f.postcode, f.city, f.address,
							f.status, f.c_sms, f.c_call, f.c_email, max(d.datereal) as donationlast, sum(dp.amount) as donationweight,
							f.archival, f.comment
						from wbz.families f
						left join wbz.distributions_positions dp
							on f.id=dp.family
						left join wbz.distributions d
							on dp.distribution=d.id
						where @filter
						group by f.id
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Family>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o rodzinie
		/// </summary>
		/// <param name="family">Klasa rodziny</param>
		internal static bool SetFamily(C_Family family)
		{
			bool result = false;
			int ID = family.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.families (declarant, lastname, members, postcode, city, address,
									status, c_sms, c_call, c_email, archival, comment)
								values (@declarant, @lastname, @members, @postcode, @city, @address,
									@status, @c_sms, @c_call, @c_email, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.families
								set declarant=@declarant, lastname=@lastname, members=@members, postcode=@postcode, city=@city, address=@address,
									status=@status, c_sms=@c_sms, c_call=@c_call, c_email=@c_email, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
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

						///add
						if (ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "families", ID, $"Utworzono rodzinę.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "families", ID, $"Edytowano rodzinę.", sqlTran);
						}

						sqlTran.Commit();
						family.ID = ID;
					}

					sqlConn.Close();
				}

				if (UpdateContacts("families", family.ID, family.Contacts))
					result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie rodziny
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteFamily(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.families where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("families", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "families", id, $"Usunięto rodzinę.");

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Distributions"
		/// <summary>
		/// Pobiera listę dystrybucji
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		/// <param name="limit">Limit rekordów</param>
		/// <param name="offset">Offset</param>
		/// <param name="order">Sortowanie po nazwie kolumny</param>
		/// <param name="desc">Porządek sortowania malejący</param>
		internal static List<C_Distribution> ListDistributions(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "datereal", bool desc = true)
		{
			var result = new List<C_Distribution>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select d.id, d.name, d.datereal, d.status,
							count(distinct dp.family) as familiescount, sum(members) as memberscount,
							count(dp.*) as positionscount, sum(dp.amount) as weight,
							d.archival, d.comment
						from wbz.distributions d
						left join wbz.distributions_positions dp
							on dp.distribution=d.id
						where @filter
						group by d.id
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_Distribution>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select 0 as id, 0 as position, 0 as store, '' as storename,
						0 as article, '' as articlename, 0.0 as amount, '' as measure", sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
						result.Rows.Clear();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera dane o pozycjach dystrybucji
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static List<C_DistributionFamily> GetDistributionPositions(int id)
		{
			var result = new List<C_DistributionFamily>();
			var dt = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}

				///przypisywanie towarów do rodzin
				foreach (DataRow row in dt.Rows)
				{
					var family = result.FirstOrDefault(x => x.Family == (int)row["family"]);
					if (family == null)
					{
						family = new C_DistributionFamily()
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
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o dystrybucji
		/// </summary>
		/// <param name="distribution">Klasa dystrybucji</param>
		internal static bool SetDistribution(C_Distribution distribution)
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
							sqlCmd = new NpgsqlCommand(@"insert into wbz.distributions (name, datereal, status, archival, comment)
								values (@name, @datereal, @status, @archival, @comment) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.distributions
								set name=@name, datereal=@datereal, status=@status, archival=@archival, comment=@comment
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("name", distribution.Name);
						sqlCmd.Parameters.AddWithValue("datereal", distribution.DateReal);
						sqlCmd.Parameters.AddWithValue("status", distribution.Status);
						sqlCmd.Parameters.AddWithValue("archival", distribution.Archival);
						sqlCmd.Parameters.AddWithValue("comment", distribution.Comment);

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
				MessageBox.Show(ex.Message);
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
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Attmisc - Attributes"
		/// <summary>
		/// Pobiera klasy atrybutów
		/// </summary>
		/// <param name="filter">Filtr SQL</param>
		internal static List<C_AttributeClass> ListAttributesClasses(string filter = null, int limit = int.MaxValue, int offset = 0, string order = "name", bool desc = false)
		{
			var result = new List<C_AttributeClass>();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select ac.id as ""ID"", ac.module as ""Module"", ac.name as ""Name"", ac.type as ""Type"", ac.""values"" as ""Values""
						from wbz.attributes_classes ac
						where @filter
						order by @order
						limit @limit offset @offset", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@order", order + " " + (desc ? "desc" : "asc"));
					sqlCmd.Parameters.AddWithValue("limit", limit);
					sqlCmd.Parameters.AddWithValue("offset", offset);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						var dt = new DataTable();
						sqlDA.Fill(dt);
						result = dt.DataTableToList<C_AttributeClass>();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Ustawia dane o klasie atrybutu
		/// </summary>
		/// <param name="attributeClass">Klasa klasy atrybutu</param>
		internal static bool SetAttributeClass(C_AttributeClass attributeClass)
		{
			bool result = false;
			int ID = attributeClass.ID;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						NpgsqlCommand sqlCmd;

						///add
						if (attributeClass.ID == 0)
							sqlCmd = new NpgsqlCommand(@"insert into wbz.attributes_classes (module, name, type, ""values"", required, archival)
								values (@module, @name, @type, @values, @required, @archival) returning id", sqlConn, sqlTran);
						///edit
						else
							sqlCmd = new NpgsqlCommand(@"update wbz.attributes_classes
								set module=@module, name=@name, type=@type, ""values""=@values, required=@required, archival=@archival
								where id=@id", sqlConn, sqlTran);

						sqlCmd.Parameters.AddWithValue("id", ID);
						sqlCmd.Parameters.AddWithValue("module", attributeClass.Module);
						sqlCmd.Parameters.AddWithValue("name", attributeClass.Name);
						sqlCmd.Parameters.AddWithValue("type", attributeClass.Type);
						sqlCmd.Parameters.AddWithValue("values", attributeClass.Values);
						sqlCmd.Parameters.AddWithValue("required", attributeClass.Required);
						sqlCmd.Parameters.AddWithValue("archival", attributeClass.Archival);

						///add
						if (attributeClass.ID == 0)
						{
							ID = Convert.ToInt32(sqlCmd.ExecuteScalar());
							SetLog(Global.User.ID, "attributes_classes", ID, $"Utworzono klasę atrybutu.", sqlTran);
						}
						///edit
						else
						{
							sqlCmd.ExecuteNonQuery();
							SetLog(Global.User.ID, "attributes_classes", ID, $"Edytowano klasę atrybutu.", sqlTran);
						}

						sqlTran.Commit();
						attributeClass.ID = ID;
					}

					sqlConn.Close();
				}

				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Usunięcie klasy atrybutu
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static bool DeleteAttributeClass(int id)
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(@"delete from wbz.attributes where class=@id;
							delete from wbz.attributes_classes where id=@id", sqlConn);
						sqlCmd.Parameters.AddWithValue("id", id);
						sqlCmd.ExecuteNonQuery();

						ClearObject("attributes_classes", id, sqlConn, sqlTran);
						SetLog(Global.User.ID, "attributes_classes", id, $"Usunięto klasę atrybutu.", sqlTran);

						sqlTran.Commit();
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Stats"
		/// <summary>
		/// Pobiera ilości towarów wg lat i miesięcy
		/// </summary>
		internal static DataTable GetStatsArticles()
		{
			var result = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"select coalesce(sum(amount),0)
						from wbz.documents i
						inner join wbz.documents_positions
							on document=i.id
						where status=1", sqlConn);
					result = Convert.ToDouble(sqlCmd.ExecuteScalar());

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(@"SELECT DISTINCT (SELECT c.name FROM wbz.companies c WHERE c.id = i.company)
						FROM wbz.documents i 
						WHERE i.archival = false and (dateissue between '@from' and '@to')", sqlConn);
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@from", from.ToString("yyyy.MM.dd"));
					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@to", to.ToString("yyyy.MM.dd 23:59:59"));
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(result);
					}

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

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

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		#endregion

		#region module "Community"
		#endregion

		#region modules
		/// <summary>
		/// Liczy ilość instancji dla danego modułu
		/// </summary>
		/// <param name="module">Nazwa modułu</param>
		/// <param name="filter">Filtr SQL</param>
		internal static int CountInstances(string module, string filter = null)
		{
			int result = 0;

			try
			{
				using (var sqlConn = new NpgsqlConnection(connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand();

					switch (module)
					{
						case Global.ModuleTypes.ARTICLES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct a.id)
								from wbz.articles a
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.ATTACHMENTS:
							sqlCmd = new NpgsqlCommand(@"select count(distinct a.id)
								from wbz.attachments a
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.ATTRIBUTES_CLASSES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct ac.id)
								from wbz.attributes_classes ac
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.COMMUNITY:
							return 0;
						case Global.ModuleTypes.COMPANIES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct c.id)
								from wbz.companies c
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.DISTRIBUTIONS:
							sqlCmd = new NpgsqlCommand(@"select count(distinct d.id)
								from wbz.distributions d
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.DOCUMENTS:
							sqlCmd = new NpgsqlCommand(@"select count(distinct d.id)
								from wbz.documents d
								left join wbz.documents_positions dp
									on dp.document=d.id
								left join wbz.companies c
									on c.id=d.company
								left join wbz.stores s
									on s.id=d.store
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.EMPLOYEES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct e.id)
								from wbz.employees e
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.FAMILIES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct f.id)
								from wbz.families f
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.LOGS:
							sqlCmd = new NpgsqlCommand(@"select count(distinct l.id)
								from wbz.logs l
								left join wbz.users u
									on l.""user"" = u.id
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.STATS:
							return 0;
						case Global.ModuleTypes.STORES:
							sqlCmd = new NpgsqlCommand(@"select count(distinct s.id)
								from wbz.stores s
								left join wbz.stores_articles sa
									on s.id = sa.store
								where @filter", sqlConn);
							break;
						case Global.ModuleTypes.USERS:
							sqlCmd = new NpgsqlCommand(@"select count(distinct u.id)
								from wbz.users u
								where @filter", sqlConn);
							break;
						default:
							return 0;
					}

					sqlCmd.CommandText = sqlCmd.CommandText.Replace("@filter", filter ?? true.ToString());
					result = Convert.ToInt32(sqlCmd.ExecuteScalar());

					sqlConn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
		/// <summary>
		/// Pobiera dane o instancji
		/// </summary>
		/// <param name="id">ID instancji</param>
		internal static object GetInstance(string module, int id)
		{
			try
			{
				switch (module)
				{
					case Global.ModuleTypes.ARTICLES:
						return ListArticles($"a.id={id}")[0];
					case Global.ModuleTypes.ATTACHMENTS:
						return null;
					case Global.ModuleTypes.ATTRIBUTES_CLASSES:
						return ListAttributesClasses($"ac.id={id}")[0];
					case Global.ModuleTypes.COMMUNITY:
						return null;
					case Global.ModuleTypes.COMPANIES:
						return ListCompanies($"c.id={id}")[0];
					case Global.ModuleTypes.DISTRIBUTIONS:
						return ListDistributions($"d.id={id}")[0];
					case Global.ModuleTypes.DOCUMENTS:
						return ListDocuments($"d.id={id}")[0];
					case Global.ModuleTypes.EMPLOYEES:
						return ListEmployees($"e.id={id}")[0];
					case Global.ModuleTypes.FAMILIES:
						return ListFamilies($"f.id={id}")[0];
					case Global.ModuleTypes.LOGS:
						return ListLogs($"l.id={id}")[0];
					case Global.ModuleTypes.STATS:
						return null;
					case Global.ModuleTypes.STORES:
						return ListStores($"s.id={id}")[0];
					case Global.ModuleTypes.USERS:
						return ListUsers($"u.id={id}")[0];
					default:
						return null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return null;
		}
		#endregion
	}
}
