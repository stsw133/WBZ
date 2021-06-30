using Newtonsoft.Json.Linq;
using StswExpress;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net.Http;
using WBZ.Globals;
using WBZ.Modules._base;
using Props = WBZ.Properties.Settings;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly D_Login D = new D_Login();

        public Login()
        {
            InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckNewestVersion();
            D.Databases = new ObservableCollection<DB>(DB.LoadAllDatabases());

            if (!string.IsNullOrEmpty(Props.Default.login_Password))
                PwdBoxPassword.Password = Props.Default.login_Password;
        }

        /// <summary>
        /// Get newest version from API URL
        /// </summary>
        private async void CheckNewestVersion()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(Props.Default.apiUrl))
                using (var content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();
                    dynamic data = JObject.Parse(result);
                    Config.VersionNewest = (string)data.versions[0];
                }

                /// check app version
                if (Fn.AppVersion() == Config.VersionNewest)
                {
                    D.StatusIcon = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/32/icon32_shield_green.ico"));
                    D.StatusName = "Posiadasz aktualną wersję programu.";
                }
                else
                {
                    D.StatusIcon = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/32/icon32_shield_orange.ico"));
                    D.StatusName = "Posiadasz nieaktualną wersję programu. Najnowsza wersja: " + Config.VersionNewest;
                }
            }
            catch
            {
                D.StatusIcon = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/32/icon32_shield_white.ico"));
                D.StatusName = null;
            }
        }

        /// <summary>
        /// Databases
        /// </summary>
        private void BtnDatabases_Click(object sender, RoutedEventArgs e)
        {
            if (new LoginDatabases() { Owner = this }.ShowDialog() == true)
                Window_Loaded(null, null);
        }

        /// <summary>
        /// Database - SelectionChanged
        /// </summary>
        private async void CmbBoxDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbDatabase = sender as ComboBox;
            if (cbDatabase.SelectedIndex < 0)
                return;

            var db = cbDatabase.SelectedItem as DB;

            Cursor = Cursors.Wait;
            try
            {
                await Task.Run(() =>
                {
                    SQL.connWBZ = StswExpress.SQL.MakeConnString(db.Server, db.Port, db.Database, db.Username, db.Password);
                    db.Version = SQL.GetPropertyValue("VERSION");
                });
                Fn.AppDatabase = db;
                BtnLogin.IsEnabled = Fn.AppDatabase.Version == Fn.AppVersion();
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"Błąd zmiany bazy:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Login
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Fn.AppDatabase.Version == null)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"Nie udało się połączyć z wybraną bazą danych!") { Owner = this }.ShowDialog();
                return;
            }
            else if (Fn.AppDatabase.Version != Fn.AppVersion())
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, $"Wersja aplikacji {Fn.AppVersion()} nie zgadza się z wersją bazy danych {Fn.AppDatabase.Version}!" +
                    Environment.NewLine + "Zaktualizuj bazę z menu dodatkowych opcji lub skontaktuj się z administratorem.") { Owner = this }.ShowDialog();
                return;
            }

            if (SQL.Login(Props.Default.login_Username, Global.sha256(PwdBoxPassword.Password)))
            {
                new Modules.Main().Show();
                Close();
            }

            if (Props.Default.login_RememberMe)
            {
                Props.Default.login_Password = PwdBoxPassword.Password;
            }
            else
            {
                Props.Default.login_Username = string.Empty;
                Props.Default.login_Password = string.Empty;
                Props.Default.login_RememberMe = false;
            }
            Props.Default.Save();
        }

        /// <summary>
        /// Opens context menu
        /// </summary>
        private void BtnContextMenu_Click(object sender, RoutedEventArgs e) => Fn.OpenContextMenu(sender);

        /// <summary>
        /// GenerateNewpass
        /// </summary>
        private void BtnGenerateNewpass_Click(object sender, RoutedEventArgs e)
        {
            var window = new MsgWin(MsgWin.Type.InputBox, "Generowanie nowego hasła", "Podaj e-mail konta, którego hasło chcesz odzyskać:") { Owner = this };
            if (window.ShowDialog() == true)
            {
                var rnd = new Random().Next(100_000, 1_000_000);
                if (Mail.SendMail(Props.Default.config_Email_Email, new string[] { window.Value },
                        "WBZ - generowanie nowego hasła", $"Kod do zmiany hasła: {rnd}"))
                    if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Wiadomość z kodem do zmiany hasła wysłano na podany e-mail.") { Owner = this }.ShowDialog() == true)
                    {
                        var cwin = new MsgWin(MsgWin.Type.InputBox, "Kod zmiany hasła", "Podaj kod otrzymany w wiadomości e-mail:") { Owner = this };
                        if (cwin.ShowDialog() == true)
                        {
                            if (cwin.Value == rnd.ToString())
                                new LoginChangePassword(window.Value) { Owner = this }.ShowDialog();
                            else
                                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Niepoprawny kod! Spróbuj wygenerować kod na nowo.") { Owner = this }.ShowDialog();
                        }
                    }
            }
        }

        /// <summary>
        /// Versions
        /// </summary>
        private void BtnVersions_Click(object sender, RoutedEventArgs e) => new Versions() { Owner = this }.ShowDialog();

        /// <summary>
        /// AboutApp
        /// </summary>
        private void BtnAboutApp_Click(object sender, RoutedEventArgs e) => new LoginAppAbout() { Owner = this }.ShowDialog();
    }
}
