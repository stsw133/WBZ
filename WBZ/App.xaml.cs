﻿using StswExpress;
using StswExpress.Translate;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Login;
using Props = WBZ.Properties.Settings;

namespace WBZ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var bindingHelp = new CommandBinding(Commands.Help, cmdHelp_Executed, cmdHelp_CanExecute);
            CommandManager.RegisterClassCommandBinding(typeof(Window), bindingHelp);

            var bindingSettings = new CommandBinding(Commands.Settings, cmdSettings_Executed, cmdSettings_CanExecute);
            CommandManager.RegisterClassCommandBinding(typeof(Window), bindingSettings);
        }

        private void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e) => Fn.OpenFile(AppDomain.CurrentDomain.BaseDirectory + @"/Resources/pl_manual.pdf");
        private void cmdHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void cmdSettings_Executed(object sender, ExecutedRoutedEventArgs e) => new Modules.Settings() { Owner = e.Source as Window }.ShowDialog();
        private void cmdSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        /// <summary>
        /// Startup
        /// </summary>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

            Settings.Default.HashKey = "ejdndbfewbasjhdggjhbasbvdgewvbjdbsavdqgwjbdjsvdyugwqyubashjdbjfgdtyuqw";

            if (Props.Default.UpgradeRequired)
            {
                Props.Default.Upgrade();
                Props.Default.UpgradeRequired = false;
                Props.Default.Save();
            }

            TM.Instance.CurrentLanguage = Props.Default.Language;
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Other\Translations.json")))
                TMLanguagesLoader.Instance.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Other\Translations.json"));

            Config.ListModules.Sort((x, y) => (x.Display as string).CompareTo(y.Display as string));

            if (DB.LoadAllDatabases().Count == 0)
                new Databases().Show();
            else
                new Login.Login().Show();
        }
    }
}
