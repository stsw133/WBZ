using StswExpress.Globals;
using StswExpress.Models;
using System;
using System.IO;
using System.Windows;
using TranslateMe;
using WBZ.Globals;
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
			StswExpress.Globals.Properties.HashKey = "ejdndbfewbasjhdggjhbasbvdgewvbjdbsavdqgwjbdjsvdyugwqyubashjdbjfgdtyuqw";
			StswExpress.Globals.Properties.iFont = WBZ.Properties.Settings.Default.iFont;
			StswExpress.Globals.Properties.iSize = WBZ.Properties.Settings.Default.iSize;
			StswExpress.Globals.Properties.Language = WBZ.Properties.Settings.Default.Language;
			StswExpress.Globals.Properties.ThemeColor = WBZ.Properties.Settings.Default.ThemeColor;

			Mail.Host = WBZ.Properties.Settings.Default.config_Email_Host;
			Mail.Port = WBZ.Properties.Settings.Default.config_Email_Port;
			Mail.Email = WBZ.Properties.Settings.Default.config_Email_Email;
			Mail.Password = WBZ.Properties.Settings.Default.config_Email_Password;
		}

		/// <summary>
		/// Startup
		/// </summary>
		private void App_Startup(object sender, StartupEventArgs e)
		{
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.tm.json")))
				TMLanguagesLoader.Instance.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.tm.json"));

			Config.ListModules.Sort((x, y) => x.Item2.CompareTo(y.Item2));

			if (M_Database.LoadAllDatabases().Count == 0)
				new LoginDatabases().Show();
			else
				new Login().Show();
		}
	}
}
