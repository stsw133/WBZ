﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WBZ.Classes;
using WBZ.Controls;
using WBZ.Helpers;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for LoginConnection.xaml
	/// </summary>
	public partial class LoginDatabases : Window
	{
		readonly M_LoginDatabases M = new M_LoginDatabases();

		public LoginDatabases()
		{
			InitializeComponent();
			DataContext = M;

			if (Owner == null)
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		/// <summary>
		/// Add database
		/// </summary>
		private void btnAddDatabase_Click(object sender, RoutedEventArgs e)
		{
			var db = new C_Database()
			{
				Name = $"db_{M.Databases.Count + 1}"
			};
			M.Databases.Add(db);
			lbDatabases.SelectedIndex = M.Databases.Count - 1;
		}

		/// <summary>
		/// Remove database
		/// </summary>
		private void btnRemoveDatabase_Click(object sender, RoutedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
				return;

			M.Databases.RemoveAt(lbDatabases.SelectedIndex);
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

			pbPassword.Password = (lbDatabases.SelectedItem as C_Database).Password;
			lblStatus.Content = "";

			gridDatabaseInfo.IsEnabled = true;
		}

		/// <summary>
		/// Password - PasswordChanged
		/// </summary>
		private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (lbDatabases.SelectedIndex == -1)
				return;

			(lbDatabases.SelectedItem as C_Database).Password = (sender as PasswordBox).Password;
		}

		/// <summary>
		/// Test
		/// </summary>
		private void btnTest_Click(object sender, RoutedEventArgs e)
		{
			SQL.connWBZ = SQL.MakeConnString(tbServer.Text, Convert.ToInt32(tbPort.Text), tbDatabase.Text, tbUsername.Text, pbPassword.Password);
			string dbv = SQL.GetPropertyValue("VERSION");

			if (dbv == Global.Version)
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
				C_Database.SaveAllDatabases(new List<C_Database>(M.Databases));
				if (Owner == null && M.Databases.Count > 0)
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

	/// <summary>
	/// Model
	/// </summary>
	internal class M_LoginDatabases : INotifyPropertyChanged
	{
		/// Databases list
		private ObservableCollection<C_Database> databases = new ObservableCollection<C_Database>(C_Database.LoadAllDatabases());
		public ObservableCollection<C_Database> Databases
		{
			get
			{
				return databases;
			}
			set
			{
				databases = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}

		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
