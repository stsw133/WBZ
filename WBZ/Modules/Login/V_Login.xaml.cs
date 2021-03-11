using Newtonsoft.Json.Linq;
using StswExpress.Globals;
using StswExpress.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Net.Http;
using WBZ.Controls;
using WBZ.Models;
using Props = WBZ.Properties.Settings;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		readonly D_Login D = new D_Login();

		public Login()
		{
			InitializeComponent();
			DataContext = D;

			if (Props.Default.UpgradeRequired)
			{
				Props.Default.Upgrade();
				Props.Default.UpgradeRequired = false;
				Props.Default.Save();
			}
		}
		
		/// <summary>
		/// Loaded
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CheckNewestVersion();
			txtVersion.Content = Global.AppVersion();

			D.Databases = new ObservableCollection<M_Database>(M_Database.LoadAllDatabases());

			if (!string.IsNullOrEmpty(Props.Default.userPass))
                tbPassword.Password = Props.Default.userPass;
		}

		/// <summary>
		/// Get newest version from API URL
		/// </summary>
		private async void CheckNewestVersion()
		{
			try
			{
				using (HttpClient client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(Props.Default.apiUrl))
				using (HttpContent content = response.Content)
				{
					string result = await content.ReadAsStringAsync();
					dynamic data = JObject.Parse(result);
					Globals.Global.VersionNewest = (string)data.versions[0];
				}

				/// check app version
				if (Global.AppVersion() == Globals.Global.VersionNewest)
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_circlegreen.ico"));
					imgVersion.ToolTip = "Posiadasz aktualną wersję programu.";
				}
				else
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_circleorange.ico"));
					imgVersion.ToolTip = "Posiadasz nieaktualną wersję programu. Najnowsza wersja: " + Globals.Global.VersionNewest;
				}
			}
			catch { }
		}

		/// <summary>
		/// Databases
		/// </summary>
		private void btnDatabases_Click(object sender, RoutedEventArgs e)
		{
			if (new LoginDatabases() { Owner = this }.ShowDialog() == true)
				Window_Loaded(null, null);
		}

		/// <summary>
		/// Database - SelectionChanged
		/// </summary>
		[STAThread]
		private async void cbDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			btnLogin.IsEnabled = false;
			btnOther.IsEnabled = false;

			var cbDatabase = (sender as ComboBox);
			if (cbDatabase.SelectedIndex < 0)
				return;

			var d = Application.Current.Dispatcher;
			await d.BeginInvoke((Action)SelectionChanged);

			void SelectionChanged()
			{
				try
				{
					var db = Global.AppDatabase = cbDatabase.SelectedItem as M_Database;
					SQL.connWBZ = StswExpress.Globals.SQL.MakeConnString(db.Server, db.Port, db.Database, db.Username, db.Password);
					Global.AppDatabase.Version = SQL.GetPropertyValue("VERSION");

					btnLogin.IsEnabled = true;
					btnOther.IsEnabled = true;
				}
				catch (Exception ex)
				{
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd zmiany bazy: " + ex.Message) { Owner = this }.ShowDialog();
				}
			}
		}

		/// <summary>
		/// Login
		/// </summary>
		private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
			if (Global.AppDatabase.Version != Global.AppVersion())
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.WARNING, $"Wersja aplikacji {Global.AppVersion()} nie zgadza się z wersją bazy danych {Global.AppDatabase.Version}!" +
					Environment.NewLine + "Zaktualizuj bazę z menu dodatkowych opcji lub skontaktuj się z administratorem.") { Owner = this }.ShowDialog();
				return;
			}

			if (SQL.Login(tbLogin.Text, Globals.Global.sha256(tbPassword.Password)))
			{
				M_Config.LoadConfig();
				new Main().Show();
				Close();
			}

			if (Props.Default.rememberMe)
				Props.Default.userPass = tbPassword.Password;
			else
			{
				Props.Default.userName = string.Empty;
				Props.Default.userPass = string.Empty;
				Props.Default.rememberMe = false;
			}
			Props.Default.Save();
		}

		/// <summary>
		/// Other
		/// </summary>
		private void btnOther_Click(object sender, RoutedEventArgs e)
		{
			///conditions of button "Dodaj administratora"
			if (SQL.CountInstances(Globals.Global.Module.USERS, @"u.blocked=false and u.archival=false and exists(select from wbz.users_permissions where ""user""=u.id and perm='admin')") == 0)
			{
				btnCreateAdmin.Visibility = Visibility.Visible;
				btnCreateAdmin.IsEnabled = true;
			}
			else
			{
				btnCreateAdmin.Visibility = Visibility.Collapsed;
				btnCreateAdmin.IsEnabled = false;
			}

			///conditions of button "Aktualizuj bazę danych"
			if (Global.AppDatabase.Version != Global.AppVersion())
			{
				btnUpdateDatabase.Visibility = Visibility.Visible;
				btnUpdateDatabase.IsEnabled = true;
				btnUpdateDatabase.Header = $"Aktualizuj bazę danych ({Global.AppDatabase.Version} → {Global.AppVersion()})";
			}
			else
			{
				btnUpdateDatabase.Visibility = Visibility.Collapsed;
				btnUpdateDatabase.IsEnabled = false;
			}

			///open context menu
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}

		/// <summary>
		/// CreateAdmin
		/// </summary>
		private void btnCreateAdmin_Click(object sender, RoutedEventArgs e)
		{
			new LoginRegister() { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// UpdateDatabase
		/// </summary>
		private void btnUpdateDatabase_Click(object sender, RoutedEventArgs e)
		{
			var conf = new Confirmation();
			conf.Owner = this;
			if (conf.ShowDialog() == true)
			{
				var users = SQL.ListInstances<Models.M_User>(Globals.Global.Module.USERS, $"(lower(username)='{conf.GetLogin.ToLower()}' or lower(email)='{conf.GetLogin.ToLower()}') and password='{Globals.Global.sha256(conf.GetPassword)}'");
				if (users.Count == 0 || !SQL.GetUserPerms(users[0].ID).Contains("admin"))
				{
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Brak uprawnień administracyjnych lub błędne dane użytkownika!") { Owner = this }.ShowDialog();
					return;
				}

				if (SQL_Migration.DoWork())
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Migracja przeprowadzona pomyślnie.") { Owner = this }.ShowDialog();
				else
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nie udało się przeprowadzić migracji!") { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// GenerateNewpass
		/// </summary>
		private void btnGenerateNewpass_Click(object sender, RoutedEventArgs e)
		{
			var window = new MsgWin(MsgWin.Type.InputBox, "Generowanie nowego hasła", "Podaj e-mail konta, którego hasło chcesz odzyskać:");
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				var rnd = new Random().Next(100_000, 1_000_000);
				if (Mail.SendMail(Props.Default.config_Email_Email, new string[] { window.Value },
						"WBZ - generowanie nowego hasła", $"Kod do zmiany hasła:{rnd}"))
					if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Wiadomość z kodem do zmiany hasła wysłano na podany e-mail.") { Owner = this }.ShowDialog() == true)
					{
						var cwin = new MsgWin(MsgWin.Type.InputBox, "Kod zmiany hasła", "Podaj kod otrzymany w wiadomości e-mail:");
						cwin.Owner = this;
						if (cwin.ShowDialog() == true)
                        {
							if (cwin.Value == rnd.ToString())
							{
								new LoginChangePassword(window.Value) { Owner = this }.ShowDialog();
							}
							else
								new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Niepoprawny kod! Spróbuj wygenerować kod na nowo.") { Owner = this }.ShowDialog();
						}
					}
			}
		}

		/// <summary>
		/// Manual
		/// </summary>
		private void btnManual_Click(object sender, RoutedEventArgs e)
		{
			Globals.Functions.OpenHelp(this);
		}

		/// <summary>
		/// Versions
		/// </summary>
		private void btnVersions_Click(object sender, RoutedEventArgs e)
		{
			new Versions() { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Settings
		/// </summary>
		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			if (new Settings() { Owner = this }.ShowDialog() == true)
            {
				new Login().Show();
				Close();
			}
		}

		/// <summary>
		/// AboutApp
		/// </summary>
		private void btnAboutApp_Click(object sender, RoutedEventArgs e)
		{
			new LoginAppAbout() { Owner = this }.ShowDialog();
		}
	}
}
