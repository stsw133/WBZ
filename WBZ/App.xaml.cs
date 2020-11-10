using System.Windows;
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
			///HELP
			//var help = new CommandBinding(Commands.Help, Commands.Help_Executed, Commands.Help_CanExecute);
			//CommandManager.RegisterClassCommandBinding(typeof(Window), help);
		}

		/// <summary>
		/// Startup
		/// </summary>
		private void App_Startup(object sender, StartupEventArgs e)
		{
			if (C_Database.LoadAllDatabases().Count == 0)
				new LoginDatabases().Show();
			else
				new Login().Show();
		}
	}
}
