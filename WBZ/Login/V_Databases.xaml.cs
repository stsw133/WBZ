using StswExpress;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for Databases.xaml
    /// </summary>
    public partial class Databases : Window
    {
        readonly D_Databases D = new D_Databases();

        public Databases()
        {
            InitializeComponent();
            DataContext = D;

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Add database
        /// </summary>
        private void BtnAddDatabase_Click(object sender, RoutedEventArgs e)
        {
            var db = new DB()
            {
                Name = $"db_{D.Databases.Count + 1}"
            };
            D.Databases.Add(db);
            LstBoxDatabases.SelectedIndex = D.Databases.Count - 1;
        }

        /// <summary>
        /// Remove database
        /// </summary>
        private void BtnRemoveDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (LstBoxDatabases.SelectedIndex < 0)
                return;

            D.Databases.RemoveAt(LstBoxDatabases.SelectedIndex);
            LstBoxDatabases.SelectedIndex = -1;
        }

        /// <summary>
        /// Databases - SelectionChanged
        /// </summary>
        private void LstBoxDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstBoxDatabases.SelectedIndex < 0)
            {
                GrdDatabaseInfo.IsEnabled = false;
                return;
            }

            PwdBoxPassword.Password = (LstBoxDatabases.SelectedItem as DB).Password;
            LblStatus.Content = string.Empty;

            GrdDatabaseInfo.IsEnabled = true;
            D.CanUpdateDatabase = false;
            D.CanCreateAdmin = false;
        }

        /// <summary>
        /// Password - PasswordChanged
        /// </summary>
        private void PwdBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (LstBoxDatabases.SelectedIndex < 0)
                return;

            (LstBoxDatabases.SelectedItem as DB).Password = (sender as PasswordBox).Password;
        }

        /// <summary>
        /// Test
        /// </summary>
        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SQL.connWBZ = StswExpress.SQL.MakeConnString(TxtBoxServer.Text, Convert.ToInt32(TxtBoxPort.Text), TxtBoxDatabase.Text, TxtBoxUsername.Text, PwdBoxPassword.Password);
                string dbv = SQL.GetPropertyValue("VERSION");

                if (dbv == null)
                {
                    LblStatus.Content = "Połączenie z bazą nie powiodło się!";
                    LblStatus.Foreground = Brushes.Red;

                    D.CanUpdateDatabase = false;
                    D.CanCreateAdmin = false;
                }
                else if (dbv == Fn.AppVersion())
                {
                    LblStatus.Content = "Wersja bazy aktualna!";
                    LblStatus.Foreground = Brushes.Green;

                    D.CanUpdateDatabase = false;
                    D.CanCreateAdmin = SQL.CountInstances(Config.GetModule(nameof(Modules.Users)), @"use.is_blocked=false and use.is_archival=false and exists(select from wbz.users_permissions where user_id=use.id and perm='Admin')") == 0;
                }
                else
                {
                    LblStatus.Content = "Wersja bazy nieaktualna!";
                    LblStatus.Foreground = Brushes.Orange;

                    D.CanUpdateDatabase = true;
                    D.CanCreateAdmin = false;
                }
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd testowania połączenia:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
        }

        /// <summary>
        /// UpdateDatabase
        /// </summary>
        private void BtnUpdateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbv = SQL.GetPropertyValue("VERSION");
                var conf = new Confirmation { Owner = this };
                if (string.IsNullOrEmpty(dbv) || conf.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(dbv))
                    {
                        var users = SQL.ListInstances<Models.M_User>(Config.GetModule(nameof(Modules.Users)), $"login='{conf.GetLogin.ToLower()}' and password='{Global.sha256(conf.GetPassword)}'");
                        if (users.Count == 0 || !SQL.GetUserPerms(users[0].ID).Contains("Admin"))
                        {
                            new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Brak uprawnień administracyjnych lub błędne dane użytkownika!") { Owner = this }.ShowDialog();
                            return;
                        }
                    }

                    if (SQL_Migration.DoWork())
                        new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.INFO, "Migracja przeprowadzona pomyślnie.") { Owner = this }.ShowDialog();
                    else
                        new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Nie udało się przeprowadzić migracji!") { Owner = this }.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd przeprowadzania migracji:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
        }

        /// <summary>
        /// CreateAdmin
        /// </summary>
        private void BtnCreateAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (new Register() { Owner = this }.ShowDialog() == true)
                D.CanCreateAdmin = false;
        }

        /// <summary>
        /// Save
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DB.SaveAllDatabases(new List<DB>(D.Databases));

                if (Owner != null)
                    DialogResult = true;
                else if (D.Databases.Count > 0)
                {
                    new Login().Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd zapisywania zmian:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
        }
    }
}
