using System;
using System.IO;
using System.Windows;
using TranslateMe;
using WBZ.Models;
using WBZ.Modules.Login;

namespace WBZ
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
        {
			StswExpress.Globals.Mail.Host = WBZ.Properties.Settings.Default.config_Email_Host;
			StswExpress.Globals.Mail.Port = WBZ.Properties.Settings.Default.config_Email_Port;
			StswExpress.Globals.Mail.Email = WBZ.Properties.Settings.Default.config_Email_Email;
			StswExpress.Globals.Mail.Password = WBZ.Properties.Settings.Default.config_Email_Password;
			StswExpress.Globals.Properties.iSize = WBZ.Properties.Settings.Default.iSize;
			StswExpress.Globals.Properties.Language = WBZ.Properties.Settings.Default.Language;
        }

		/// <summary>
		/// Startup
		/// </summary>
		private void App_Startup(object sender, StartupEventArgs e)
		{
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.tm.json")))
				TMLanguagesLoader.Instance.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.tm.json"));

			if (M_Database.LoadAllDatabases().Count == 0)
				new LoginDatabases().Show();
			else
				new Login().Show();
		}
	}
}
