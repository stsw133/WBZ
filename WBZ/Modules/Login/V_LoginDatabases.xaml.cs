using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WBZ.Controls;
using StswExpress.Models;

namespace WBZ.Modules.Login
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
			var db = new M_Database()
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

			pbPassword.Password = (lbDatabases.SelectedItem as M_Database).Password;
			lblStatus.Content = string.Empty;

			gridDatabaseInfo.IsEnabled = true;
		}

		/// <summary>
		/// Password - PasswordChanged
		/// </summary>
		private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
				return;

			(lbDatabases.SelectedItem as M_Database).Password = (sender as PasswordBox).Password;
		}

		/// <summary>
		/// Test
		/// </summary>
		private void btnTest_Click(object sender, RoutedEventArgs e)
		{
			SQL.connWBZ = StswExpress.Globals.SQL.MakeConnString(tbServer.Text, Convert.ToInt32(tbPort.Text), tbDatabase.Text, tbUsername.Text, pbPassword.Password);
			string dbv = SQL.GetPropertyValue("VERSION");

			if (dbv == StswExpress.Globals.Global.AppVersion())
			{
				lblStatus.Content = "Wersja bazy aktualna!";
				lblStatus.Foreground = Brushes.Green;
			}
			else if (dbv == null)
			{
				lblStatus.Content = "Połączenie z bazą nie powiodło się!";
				lblStatus.Foreground = Brushes.Red;
			}
			else
			{
				lblStatus.Content = "Wersja bazy nieaktualna!";
				lblStatus.Foreground = Brushes.Orange;
			}
		}

		/// <summary>
		/// Save
		/// </summary>
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				M_Database.SaveAllDatabases(new List<M_Database>(D.Databases));
				if (Owner == null && D.Databases.Count > 0)
				{
					var window = new Login();
					window.Show();
				}

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
