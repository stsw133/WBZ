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
		/// <summary>
		/// Startup
		/// </summary>
		private void App_Startup(object sender, StartupEventArgs e)
		{
			TMLanguagesLoader.Instance.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.tm.json"));

			if (M_Database.LoadAllDatabases().Count == 0)
				new LoginDatabases().Show();
			else
				new Login().Show();
		}
	}
}
