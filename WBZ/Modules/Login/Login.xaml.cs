using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Reflection;
using WBZ.Classes;
using WBZ.Controls;
using WBZ.Helpers;
using Props = WBZ.Properties.Settings;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		readonly M_Login M = new M_Login();

		public Login()
		{
			InitializeComponent();
			DataContext = M;

			if (Props.Default.UpgradeRequired)
			{
				Props.Default.Upgrade();
				Props.Default.UpgradeRequired = false;
				Props.Default.Save();
			}
		}
		
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			///sprawdzanie najnowszej wersji
			CheckNewestVersion();
			txtVersion.Content = Global.Version;

			///wczytanie listy baz danych
			M.Databases = new ObservableCollection<C_Database>(C_Database.LoadAllDatabases());

			///wpisanie hasła jeśli ustawiona opcja "Zapamiętaj dane logowania"
			if (!string.IsNullOrEmpty(Props.Default.userPass))
                tbPassword.Password = Props.Default.userPass;
		}

		/// <summary>
		/// Funkcja pobierająca numer najnowszej wersji przez API URL
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

				///sprawdzenie wersji programu
				if (Global.Version == Global.VersionNewest)
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_fine.ico"));
					imgVersion.ToolTip = "Posiadasz aktualną wersję programu.";
				}
				else
				{
					imgVersion.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_warning.ico"));
					imgVersion.ToolTip = "Posiadasz nieaktualną wersję programu. Najnowsza wersja to " + Global.VersionNewest;
				}
			}
			catch { }
		}

		/// <summary>
		/// Przycisk zmiany połączenia z główną bazą
		/// </summary>
		private void btnDatabases_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginDatabases();
			window.Owner = this;
			if (window.ShowDialog() == true)
				Window_Loaded(null, null);
		}

		/// <summary>
		/// Moment zmiany bazy oddziału
		/// </summary>
		private void cbDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbDatabase = (sender as ComboBox);

			try
			{
				if (cbDatabase.SelectedIndex < 0)
					cbDatabase.SelectedIndex = 0;

				Global.Database = cbDatabase.SelectedItem as C_Database;
				SQL.connWBZ = SQL.MakeConnString(Global.Database.Server, Global.Database.Port, Global.Database.Database, Global.Database.Username, Global.Database.Password);
				Global.Database.Version = SQL.GetPropertyValue("VERSION");

				btnLogin.IsEnabled = true;
				btnOther.IsEnabled = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Przycisk logowania do systemu
		/// </summary>
		private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
			if (Global.Database.Version != Global.Version)
			{
				MessageBox.Show($"Wersja aplikacji {Global.Version} nie zgadza się z wersją bazy danych {Global.Database.Version}!" +
					Environment.NewLine + "Zaktualizuj bazę z menu dodatkowych opcji lub skontaktuj się z administratorem.");
				return;
			}

			if (SQL.Login(tbLogin.Text, Global.sha256(tbPassword.Password)))
			{
				C_Config.LoadConfig();
				var window = new Main();
				window.Show();
				Close();
			}

			if (Props.Default.rememberMe)
				Props.Default.userPass = tbPassword.Password;
			else
			{
				Props.Default.userName = "";
				Props.Default.userPass = "";
				Props.Default.rememberMe = false;
			}
			Props.Default.Save();
		}

		/// <summary>
		/// Przycisk pozostałych opcji
		/// </summary>
		private void btnOther_Click(object sender, RoutedEventArgs e)
		{
			/// warunki pojawienia się przycisku "Dodaj administratora"
			if (SQL.CountInstances(Global.Module.USERS, @"blocked=false and archival=false and exists(select from wbz.users_permissions where ""user""=u.id and perm='admin')") == 0)
			{
				btnCreateAdmin.Visibility = Visibility.Visible;
				btnCreateAdmin.IsEnabled = true;
			}
			else
			{
				btnCreateAdmin.Visibility = Visibility.Collapsed;
				btnCreateAdmin.IsEnabled = false;
			}

			/// warunki pojawienia się przycisku "Aktualizuj bazę danych"
			if (Global.Database.Version != Global.Version)
			{
				btnUpdateDatabase.Visibility = Visibility.Visible;
				btnUpdateDatabase.IsEnabled = true;
				btnUpdateDatabase.Header = $"Aktualizuj bazę danych ({Global.Database.Version} → {Global.Version})";
			}
			else
			{
				btnUpdateDatabase.Visibility = Visibility.Collapsed;
				btnUpdateDatabase.IsEnabled = false;
			}

			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}

		/// <summary>
		/// Przycisk rejestracji konta administracyjnego w systemie
		/// </summary>
		private void btnCreateAdmin_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginRegister();
			window.Owner = this;
			window.ShowDialog();
		}

		/// <summary>
		/// Przycisk aktualizacji bazy danych do najnowszej wersji
		/// </summary>
		private void btnUpdateDatabase_Click(object sender, RoutedEventArgs e)
		{
			var conf = new Confirmation();
			conf.Owner = this;
			if (conf.ShowDialog() == true)
			{
				var users = SQL.ListInstances(Global.Module.USERS, $"(lower(username)='{conf.GetLogin.ToLower()}' or lower(email)='{conf.GetLogin.ToLower()}') and password='{Global.sha256(conf.GetPassword)}'").DataTableToList<C_User>();
				if (users.Count == 0 || !SQL.GetUserPerms(users[0].ID).Contains("admin"))
				{
					MessageBox.Show("Brak uprawnień administracyjnych lub błędne dane użytkownika!");
					return;
				}

				if (SQL_Migration.DoWork())
					MessageBox.Show("Migracja przeprowadzona pomyślnie.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
				else
					MessageBox.Show("Nie udało się przeprowadzić migracji!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Przycisk generowania nowego hasła
		/// </summary>
		private void btnGenerateNewpass_Click(object sender, RoutedEventArgs e)
		{
			//TODO - nowy sposób generowania hasła z wysłaniem kodu na maila
			var window = new MsgWin(MsgWin.Type.InputBox, "Generowanie nowego hasła", $"Podaj e-mail konta, którego hasło chcesz odzyskać:");
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				var loginData = SQL.GenerateNewPasswordForAccount(window.values[0]);
				if (Mail.SendMail(Props.Default.config_Email_Email, new string[] { window.values[0] },
						"WBZ - generowanie nowego hasła", $"Nazwa użytkownika: {loginData[0]}{Environment.NewLine}Hasło: {loginData[1]}"))
					MessageBox.Show("Wiadomość z nazwą użytkownika i hasłem wysłano na podany e-mail.");
			}
		}

		/// <summary>
		/// Przycisk otworzenia poradnika użytkowania
		/// </summary>
		private void btnManual_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process process = new Process();
				string path = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/pl_manual.pdf";
				process.StartInfo.FileName = new Uri(path, UriKind.RelativeOrAbsolute).LocalPath;
				process.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Przycisk otworzenia okna z wersjami aplikacji
		/// </summary>
		private void btnVersions_Click(object sender, RoutedEventArgs e)
		{
			var window = new Versions();
			window.Owner = this;
			window.ShowDialog();
		}

		/// <summary>
		/// Przycisk otworzenia okna z informacjami o programie
		/// </summary>
		private void btnAboutApp_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginAppAbout();
			window.Owner = this;
			window.ShowDialog();
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_Login : INotifyPropertyChanged
	{
		/// Lista baz danych
		private ObservableCollection<C_Database> databases;
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

		/// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
