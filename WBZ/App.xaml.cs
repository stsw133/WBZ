﻿using StswExpress.Base;
using StswExpress.Globals;
using StswExpress.Translate;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
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
			ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

			StswExpress.Properties.Settings.Default.HashKey = "ejdndbfewbasjhdggjhbasbvdgewvbjdbsavdqgwjbdjsvdyugwqyubashjdbjfgdtyuqw";
			StswExpress.Properties.Settings.Default.iFont = WBZ.Properties.Settings.Default.iFont;
			StswExpress.Properties.Settings.Default.iSize = WBZ.Properties.Settings.Default.iSize;
			StswExpress.Properties.Settings.Default.ThemeColor = WBZ.Properties.Settings.Default.ThemeColor;

			Mail.Host = WBZ.Properties.Settings.Default.config_Email_Host;
			Mail.Port = WBZ.Properties.Settings.Default.config_Email_Port;
			Mail.Email = WBZ.Properties.Settings.Default.config_Email_Email;
			Mail.Password = WBZ.Properties.Settings.Default.config_Email_Password;

			TM.Instance.CurrentLanguage = WBZ.Properties.Settings.Default.Language;
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.json")))
				TMLanguagesLoader.Instance.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Translations.json"));

			Config.ListModules.Sort((x, y) => x.Item2.CompareTo(y.Item2));

			if (M_Database.LoadAllDatabases().Count == 0)
				new LoginDatabases().Show();
			else
				new Login().Show();
		}
	}
}
