using StswExpress;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WBZ.Controls;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ.Login
{
	/// <summary>
	/// Interaction logic for LoginConnection.xaml
	/// </summary>
	public partial class LoginDatabases : Window
	{
		readonly D_LoginDatabases D = new D_LoginDatabases();

		public LoginDatabases()
		{
			InitializeComponent();
			DataContext = D;

			if (Owner == null)
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		/// <summary>
		/// Add database
		/// </summary>
		private void btnAddDatabase_Click(object sender, RoutedEventArgs e)
		{
			var db = new DB()
			{
				Name = $"db_{D.Databases.Count + 1}"
			};
			D.Databases.Add(db);
			lbDatabases.SelectedIndex = D.Databases.Count - 1;
		}

		/// <summary>
		/// Remove database
		/// </summary>
		private void btnRemoveDatabase_Click(object sender, RoutedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
				return;

			D.Databases.RemoveAt(lbDatabases.SelectedIndex);
			lbDatabases.SelectedIndex = -1;
		}

		/// <summary>
		/// Databases - SelectionChanged
		/// </summary>
		private void lbDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
			{
				gridDatabaseInfo.IsEnabled = false;
				return;
			}

			pbPassword.Password = (lbDatabases.SelectedItem as DB).Password;
			lblStatus.Content = string.Empty;

			gridDatabaseInfo.IsEnabled = true;
			D.CanUpdateDatabase = false;
			D.CanCreateAdmin = false;
		}

		/// <summary>
		/// Password - PasswordChanged
		/// </summary>
		private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
				return;

			(lbDatabases.SelectedItem as DB).Password = (sender as PasswordBox).Password;
		}

		/// <summary>
		/// Test
		/// </summary>
		private void btnTest_Click(object sender, RoutedEventArgs e)
		{
			SQL.connWBZ = StswExpress.SQL.MakeConnString(tbServer.Text, Convert.ToInt32(tbPort.Text), tbDatabase.Text, tbUsername.Text, pbPassword.Password);
			string dbv = SQL.GetPropertyValue("VERSION");

			if (dbv == null)
			{
				lblStatus.Content = "Połączenie z bazą nie powiodło się!";
				lblStatus.Foreground = Brushes.Red;

				D.CanUpdateDatabase = false;
				D.CanCreateAdmin = false;
			}
			else if (dbv == Fn.AppVersion())
			{
				lblStatus.Content = "Wersja bazy aktualna!";
				lblStatus.Foreground = Brushes.Green;

				D.CanUpdateDatabase = false;
				D.CanCreateAdmin = SQL.CountInstances(Config.GetModule(nameof(Modules.Users)), @"use.blocked=false and use.archival=false and exists(select from wbz.users_permissions where ""user""=use.id and perm='Admin')") == 0;
			}
			else
			{
				lblStatus.Content = "Wersja bazy nieaktualna!";
				lblStatus.Foreground = Brushes.Orange;

				D.CanUpdateDatabase = true;
				D.CanCreateAdmin = false;
			}
		}

		/// <summary>
		/// UpdateDatabase
		/// </summary>
		private void btnUpdateDatabase_Click(object sender, RoutedEventArgs e)
		{
			string dbv = SQL.GetPropertyValue("VERSION");
			var conf = new Confirmation { Owner = this };
			if (string.IsNullOrEmpty(dbv) || conf.ShowDialog() == true)
			{
				if (!string.IsNullOrEmpty(dbv))
				{
					var users = SQL.ListInstances<Models.M_User>(Config.GetModule(nameof(Modules.Users)), $"(lower(username)='{conf.GetLogin.ToLower()}' or lower(email)='{conf.GetLogin.ToLower()}') and password='{Global.sha256(conf.GetPassword)}'");
					if (users.Count == 0 || !SQL.GetUserPerms(users[0].ID).Contains("Admin"))
					{
						new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Brak uprawnień administracyjnych lub błędne dane użytkownika!") { Owner = this }.ShowDialog();
						return;
					}
				}

				if (SQL_Migration.DoWork())
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Migracja przeprowadzona pomyślnie.") { Owner = this }.ShowDialog();
				else
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nie udało się przeprowadzić migracji!") { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// CreateAdmin
		/// </summary>
		private void btnCreateAdmin_Click(object sender, RoutedEventArgs e)
		{
			if (new LoginRegister() { Owner = this }.ShowDialog() == true)
				D.CanCreateAdmin = false;
		}

		/// <summary>
		/// Save
		/// </summary>
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				DB.SaveAllDatabases(new List<DB>(D.Databases));
				if (Owner == null && D.Databases.Count > 0)
					new Login().Show();

				if (Owner != null)
					DialogResult = true;
				else
					Close();
			}
			catch (Exception ex)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd zapisywania zmian: " + ex.Message) { Owner = this }.ShowDialog();
			}
		}
	}
}
