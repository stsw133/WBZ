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
using WBZ.Controls;
using WBZ.Modules._base;
using Props = WBZ.Properties.Settings;

namespace WBZ.Modules.Login
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
			txtVersion.Content = Fn.AppVersion();

			D.Databases = new ObservableCollection<DB>(DB.LoadAllDatabases());

			if (!string.IsNullOrEmpty(Props.Default.Login_Password))
                tbPassword.Password = Props.Default.Login_Password;
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
				if (Fn.AppVersion() == Globals.Global.VersionNewest)
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_shield_green.ico"));
					imgVersion.ToolTip = "Posiadasz aktualną wersję programu.";
				}
				else
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_shield_orange.ico"));
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
		private async void cbDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbDatabase = sender as ComboBox;
			if (cbDatabase.SelectedIndex < 0)
				return;

			var db = Fn.AppDatabase = cbDatabase.SelectedItem as DB;

			Cursor = Cursors.Wait;
			await Task.Run(() =>
			{
				try
				{
					SQL.connWBZ = StswExpress.SQL.MakeConnString(db.Server, db.Port, db.Database, db.Username, db.Password);
					Fn.AppDatabase.Version = SQL.GetPropertyValue("VERSION");
				}
				catch (Exception ex)
				{
					new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd zmiany bazy: " + ex.Message) { Owner = this }.ShowDialog();
				}
			});
			Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Login
		/// </summary>
		private void btnLogin_Click(object sender, RoutedEventArgs e)
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

			if (SQL.Login(tbLogin.Text, Globals.Global.sha256(tbPassword.Password)))
			{
				new Main().Show();
				Close();
			}

			if (Props.Default.Login_RememberMe)
			{
				Props.Default.Login_Password = tbPassword.Password;
			}
			else
			{
				Props.Default.Login_Username = string.Empty;
				Props.Default.Login_Password = string.Empty;
				Props.Default.Login_RememberMe = false;
			}
			Props.Default.Save();
		}

		/// <summary>
		/// Opens context menu
		/// </summary>
		private void btnContextMenu_Click(object sender, RoutedEventArgs e) => Fn.OpenContextMenu(sender);

		/// <summary>
		/// GenerateNewpass
		/// </summary>
		private void btnGenerateNewpass_Click(object sender, RoutedEventArgs e)
		{
			var window = new MsgWin(MsgWin.Type.InputBox, "Generowanie nowego hasła", "Podaj e-mail konta, którego hasło chcesz odzyskać:") { Owner = this };
			if (window.ShowDialog() == true)
			{
				var rnd = new Random().Next(100_000, 1_000_000);
				if (Mail.SendMail(Props.Default.config_Email_Email, new string[] { window.Value },
						"WBZ - generowanie nowego hasła", $"Kod do zmiany hasła:{rnd}"))
					if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Wiadomość z kodem do zmiany hasła wysłano na podany e-mail.") { Owner = this }.ShowDialog() == true)
					{
						var cwin = new MsgWin(MsgWin.Type.InputBox, "Kod zmiany hasła", "Podaj kod otrzymany w wiadomości e-mail:")
						{
							Owner = this
						};
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
		private void btnVersions_Click(object sender, RoutedEventArgs e) => new Versions() { Owner = this }.ShowDialog();

		/// <summary>
		/// AboutApp
		/// </summary>
		private void btnAboutApp_Click(object sender, RoutedEventArgs e) => new LoginAppAbout() { Owner = this }.ShowDialog();
	}
}
