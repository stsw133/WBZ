using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WBZ.Helpers;

namespace WBZ.Classes
{
	public class C_Database
	{
		public string Name { get; set; }
		public string Server { get; set; }
		public int Port { get; set; }
		public string Database { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Version { get; set; }

		public C_Database()
		{
			Name = "";
			Server = "";
			Port = 0;
			Database = "";
			Username = "";
			Password = "";
			Version = "";
		}

		/// <summary>
		/// Wczytanie listy baz danych z pliku
		/// </summary>
		public static List<C_Database> LoadAllDatabases()
		{
			var result = new List<C_Database>();
			var db = new C_Database();

			try
			{
				if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Databases.bin")))
				{
					Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
					File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Databases.bin")).Close();
				}

				using (var stream = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Databases.bin")))
				{
					while (!stream.EndOfStream)
					{
						string line = stream.ReadLine();
						string property = line.Split('=')[0];

						if (property == "Name")
						{
							db = new C_Database();
							db.Name = Global.Decrypt(line.Substring(property.Length + 1));
						}
						else if (property == "Server")
							db.Server = Global.Decrypt(line.Substring(property.Length + 1));
						else if (property == "Port")
							db.Port = Convert.ToInt32(Global.Decrypt(line.Substring(property.Length + 1)));
						else if (property == "Database")
							db.Database = Global.Decrypt(line.Substring(property.Length + 1));
						else if (property == "Username")
							db.Username = Global.Decrypt(line.Substring(property.Length + 1));
						else if (property == "Password")
						{
							db.Password = Global.Decrypt(line.Substring(property.Length + 1));
							result.Add(db);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Błąd wczytywania listy baz danych: " + ex.Message);
			}

			return result;
		}

		/// <summary>
		/// Zapisanie listy baz danych do pliku
		/// </summary>
		public static void SaveAllDatabases(List<C_Database> databases)
		{
			try
			{
				using (var stream = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Databases.bin")))
				{
					foreach (C_Database db in databases)
					{
						stream.WriteLine($"Name={Global.Encrypt(db.Name)}");
						stream.WriteLine($"Server={Global.Encrypt(db.Server)}");
						stream.WriteLine($"Port={Global.Encrypt(db.Port.ToString())}");
						stream.WriteLine($"Database={Global.Encrypt(db.Database)}");
						stream.WriteLine($"Username={Global.Encrypt(db.Username)}");
						stream.WriteLine($"Password={Global.Encrypt(db.Password)}");
						stream.WriteLine("");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Błąd zapisywania listy baz danych: " + ex.Message);
			}
		}
	}
}
