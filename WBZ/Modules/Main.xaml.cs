using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Controls;
using WBZ.Globals;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Attachments;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Logs;
using WBZ.Modules.Personal;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;

namespace WBZ.Modules
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		readonly D_Main D = new D_Main();

		public Main()
		{
			InitializeComponent();
			DataContext = D;
		}

		/// <summary>
		/// Loaded
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			///USERS
			if (!Global.User.Perms.Contains($"{Global.Module.USERS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modUsers);
			///STORES
			if (!Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modStores);
			///STATS
			if (!Global.User.Perms.Contains($"{Global.Module.STATS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modStats);
			///LOGS
			if (!Global.User.Perms.Contains($"{Global.Module.LOGS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modLogs);
			///FAMILIES
			if (!Global.User.Perms.Contains($"{Global.Module.FAMILIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modFamilies);
			///EMPLOYEES
			if (!Global.User.Perms.Contains($"{Global.Module.EMPLOYEES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modEmployees);
			///DOCUMENTS
			if (!Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modDocuments);
			///DISTRIBUTIONS
			if (!Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modDistributions);
			///COMPANIES
			if (!Global.User.Perms.Contains($"{Global.Module.COMPANIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modCompanies);
			///ATTRIBUTES_CLASSES
			if (!Global.User.Perms.Contains($"{Global.Module.ATTRIBUTES_CLASSES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modAttributesClasses);
			///ATTACHMENTS
			if (!Global.User.Perms.Contains($"{Global.Module.ATTACHMENTS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modAttachments);
			///ARTICLES
			if (!Global.User.Perms.Contains($"{Global.Module.ARTICLES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modArticles);
			///ADMIN
			if (!Global.User.Perms.Contains($"admin"))
				gridModules.Children.Remove(modAdmin);
		}

		/// <summary>
		/// Menu
		/// </summary>
		private void menuHelp_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Functions.OpenHelp(this);
		}
		private void menuSettings_Click(object sender, RoutedEventArgs e)
		{
			new Settings() { Owner = this }.ShowDialog();
		}
		private void menuRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Global.User = SQL.GetInstance<M_User>(Global.Module.USERS, Global.User.ID);
			Global.User.Perms = SQL.GetUserPerms(Global.User.ID);

			var window = new Main();
			window.Show();

			D.WantToLogout = true;
			Close();
		}
		private void menuLogout_Click(object sender, RoutedEventArgs e)
		{
			if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.CONFIRMATION, "Na pewno wylogować?") { Owner = this }.ShowDialog() == true)
			{
				Global.User = new M_User();

				foreach (Window x in App.Current.Windows)
					if (x != this)
						x.Close();

				var window = new Login.Login();
				window.Show();

				D.WantToLogout = true;
				Close();
			}
		}
		private void menuClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Personal - open context menu
		/// </summary>
		private void btnOther_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}

		/// <summary>
		/// Personal
		/// </summary>
		private void btnProfile_Click(object sender, RoutedEventArgs e)
		{
			new Profile() { Owner = this }.ShowDialog();
		}
		private void btnCalendar_Click(object sender, RoutedEventArgs e)
		{
			//new Calendar() { Owner = this }.ShowDialog();
		}
		private void btnMail_Click(object sender, RoutedEventArgs e)
		{
			//new Mail() { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Search - TextChanged
		/// </summary>
		private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			var searchText = (sender as TextBox).Text.ToLower();

			foreach (Border module in gridModules.Children)
			{
				var dp = (module.Child as DockPanel);
				var bord = (dp.Children[0] as Border);
				var sp = (bord.Child as StackPanel);
				var lab = (sp.Children[1] as Label);
				var moduleName = lab.Content.ToString().ToLower();

				if (moduleName.Contains(searchText.ToLower()))
					module.Visibility = Visibility.Visible;
				else
					module.Visibility = Visibility.Collapsed;
			}
		}
		
		/// <summary>
		/// Admin
		/// </summary>
		private void btnCommunity_Click(object sender, MouseButtonEventArgs e)
		{
			//new Forum() { Owner = this }.ShowDialog();
		}
		private void btnConsoleSQL_Click(object sender, MouseButtonEventArgs e)
		{
			new ConsoleSQL() { Owner = this }.Show();
		}

		/// <summary>
		/// Articles
		/// </summary>
		private void btnArticlesList_Click(object sender, MouseButtonEventArgs e)
		{
			new ArticlesList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnArticlesNew_Click(object sender, MouseButtonEventArgs e)
		{
			new ArticlesNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Attachments
		/// </summary>
		private void btnAttachmentsList_Click(object sender, MouseButtonEventArgs e)
		{
			new AttachmentsList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnAttachmentsGallery_Click(object sender, MouseButtonEventArgs e)
		{
			new AttachmentsGallery() { Owner = this }.Show();
		}

		/// <summary>
		/// AttributesClasses
		/// </summary>
		private void btnAttributesClassesList_Click(object sender, MouseButtonEventArgs e)
		{
			new AttributesClassesList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnAttributesClassesNew_Click(object sender, MouseButtonEventArgs e)
		{
			new AttributesClassesNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Companies
		/// </summary>
		private void btnCompaniesList_Click(object sender, MouseButtonEventArgs e)
		{
			new CompaniesList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnCompaniesNew_Click(object sender, MouseButtonEventArgs e)
		{
			new CompaniesNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Distributions
		/// </summary>
		private void btnDistributionsList_Click(object sender, MouseButtonEventArgs e)
		{
			new DistributionsList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnDistributionsNew_Click(object sender, MouseButtonEventArgs e)
		{
			new DistributionsNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Documents
		/// </summary>
		private void btnDocumentsList_Click(object sender, MouseButtonEventArgs e)
		{
			new DocumentsList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnDocumentsNew_Click(object sender, MouseButtonEventArgs e)
		{
			new DocumentsNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Employees
		/// </summary>
		private void btnEmployeesList_Click(object sender, MouseButtonEventArgs e)
		{
			new EmployeesList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnEmployeesNew_Click(object sender, MouseButtonEventArgs e)
		{
			new EmployeesNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Families
		/// </summary>
		private void btnFamiliesList_Click(object sender, MouseButtonEventArgs e)
		{
			new FamiliesList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnFamiliesNew_Click(object sender, MouseButtonEventArgs e)
		{
			new FamiliesNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Logs
		/// </summary>
		private void btnLogsList_Click(object sender, MouseButtonEventArgs e)
		{
			new LogsList() { Owner = this }.Show();
		}

		/// <summary>
		/// Stats
		/// </summary>
		private void btnStats_Click(object sender, MouseButtonEventArgs e)
		{
			new Stats.Stats() { Owner = this }.Show();
		}

		/// <summary>
		/// Stores
		/// </summary>
		private void btnStoresList_Click(object sender, MouseButtonEventArgs e)
		{
			new StoresList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnStoresNew_Click(object sender, MouseButtonEventArgs e)
		{
			new StoresNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Users
		/// </summary>
		private void btnUsersList_Click(object sender, MouseButtonEventArgs e)
		{
			new UsersList(Commands.Type.LIST) { Owner = this }.Show();
		}
		private void btnUsersNew_Click(object sender, MouseButtonEventArgs e)
		{
			new UsersNew(null, Commands.Type.NEW) { Owner = this }.Show();
		}

		/// <summary>
		/// Closing
		/// </summary>
		private void Window_Closing(object sender, CancelEventArgs e)
        {
			Properties.Settings.Default.Save();
            if (!D.WantToLogout)
                foreach (Window x in App.Current.Windows)
                    if (x != this)
                        x.Close();
        }
	}
}
