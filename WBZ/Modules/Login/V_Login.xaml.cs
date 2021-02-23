using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Net.Http;
using WBZ.Controls;
using WBZ.Globals;
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
			txtVersion.Content = StswExpress.Globals.Global.AppVersion();

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
					Global.VersionNewest = (string)data.versions[0];
				}

				/// check app version
				if (StswExpress.Globals.Global.AppVersion() == Global.VersionNewest)
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_circlegreen.ico"));
					imgVersion.ToolTip = "Posiadasz aktualną wersję programu.";
				}
				else
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_circleorange.ico"));
					imgVersion.ToolTip = "Posiadasz nieaktualną wersję programu. Najnowsza wersja to " + Global.VersionNewest;
				}
			}
			catch { }
		}

		/// <summary>
		/// Databases
		/// </summary>
		private void btnDatabases_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginDatabases();
			window.Owner = this;
			if (window.ShowDialog() == true)
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
					Global.Database = cbDatabase.SelectedItem as M_Database;
					SQL.connWBZ = SQL.MakeConnString(Global.Database.Server, Global.Database.Port, Global.Database.Database, Global.Database.Username, Global.Database.Password);
					Global.Database.Version = SQL.GetPropertyValue("VERSION");

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
			if (Global.Database.Version != StswExpress.Globals.Global.AppVersion())
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.WARNING, $"Wersja aplikacji {StswExpress.Globals.Global.AppVersion()} nie zgadza się z wersją bazy danych {Global.Database.Version}!" +
					Environment.NewLine + "Zaktualizuj bazę z menu dodatkowych opcji lub skontaktuj się z administratorem.") { Owner = this }.ShowDialog();
				return;
			}

			if (SQL.Login(tbLogin.Text, Global.sha256(tbPassword.Password)))
			{
				M_Config.LoadConfig();
				var window = new Main();
				window.Show();
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
			if (SQL.CountInstances(Global.Module.USERS, @"u.blocked=false and u.archival=false and exists(select from wbz.users_permissions where ""user""=u.id and perm='admin')") == 0)
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
			if (Global.Database.Version != StswExpress.Globals.Global.AppVersion())
			{
				btnUpdateDatabase.Visibility = Visibility.Visible;
				btnUpdateDatabase.IsEnabled = true;
				btnUpdateDatabase.Header = $"Aktualizuj bazę danych ({Global.Database.Version} → {StswExpress.Globals.Global.AppVersion()})";
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
			var window = new LoginRegister();
			window.Owner = this;
			window.ShowDialog();
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
				var users = SQL.ListInstances<M_User>(Global.Module.USERS, $"(lower(username)='{conf.GetLogin.ToLower()}' or lower(email)='{conf.GetLogin.ToLower()}') and password='{Global.sha256(conf.GetPassword)}'");
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
			//TODO - nowy sposób generowania hasła z wysłaniem kodu na maila
			var window = new MsgWin(MsgWin.Type.InputBox, "Generowanie nowego hasła", "Podaj e-mail konta, którego hasło chcesz odzyskać:");
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				var loginData = SQL.GenerateNewPasswordForAccount(window.Value);
				if (StswExpress.Globals.Mail.SendMail(Props.Default.config_Email_Email, new string[] { window.Value },
						"WBZ - generowanie nowego hasła", $"Nazwa użytkownika: {loginData[0]}{Environment.NewLine}Hasło: {loginData[1]}"))
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Wiadomość z nazwą użytkownika i hasłem wysłano na podany e-mail.") { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// Manual
		/// </summary>
		private void btnManual_Click(object sender, RoutedEventArgs e)
		{
			//TODO - naprawić
			Functions.OpenHelp(this);
		}

		/// <summary>
		/// Versions
		/// </summary>
		private void btnVersions_Click(object sender, RoutedEventArgs e)
		{
			var window = new Versions();
			window.Owner = this;
			window.ShowDialog();
		}

		/// <summary>
		/// Settings
		/// </summary>
		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			var window = new Settings();
			window.Owner = this;
			if (window.ShowDialog() == true)
            {
				var login = new Login();
				login.Show();
				Close();
			}
		}

		/// <summary>
		/// AboutApp
		/// </summary>
		private void btnAboutApp_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginAppAbout();
			window.Owner = this;
			window.ShowDialog();
		}
	}
}
