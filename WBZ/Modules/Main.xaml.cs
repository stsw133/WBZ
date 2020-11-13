using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Controls;
using WBZ.Helpers;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Personal;
using WBZ.Modules.Stores;
using WBZ.Modules.Groups;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Attachments;
using WBZ.Modules.Logs;

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
			///GROUPS
			if (!Global.User.Perms.Contains($"{Global.Module.GROUPS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modGroups);
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
		/// Menu - Help
		/// </summary>
		private void menuHelp_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Functions.OpenWindow_Help(this);
		}

		/// <summary>
		/// Menu - Settings
		/// </summary>
		private void menuSettings_Click(object sender, RoutedEventArgs e)
		{
			Functions.OpenWindow_Settings(this);
		}

		/// <summary>
		/// Menu - Refresh
		/// </summary>
		private void menuRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Global.User = SQL.GetInstance<C_User>(Global.Module.USERS, Global.User.ID);
			Global.User.Perms = SQL.GetUserPerms(Global.User.ID);

			var window = new Main();
			window.Show();

			D.WantToLogout = true;
			Close();
		}

		/// <summary>
		/// Menu - Logout
		/// </summary>
		private void menuLogout_Click(object sender, RoutedEventArgs e)
		{
			if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.CONFIRMATION, "Na pewno wylogować?") { Owner = this }.ShowDialog() == true)
			{
				Global.User = new C_User();

				foreach (Window x in App.Current.Windows)
					if (x != this)
						x.Close();

				var window = new Login.Login();
				window.Show();

				D.WantToLogout = true;
				Close();
			}
		}

		/// <summary>
		/// Menu - Close
		/// </summary>
		private void menuClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Other - open context menu
		/// </summary>
		private void btnOther_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}

		/// <summary>
		/// Profile
		/// </summary>
		private void btnProfile_Click(object sender, RoutedEventArgs e)
		{
			var window = new Profile();
			window.ShowDialog();
		}

		/// <summary>
		/// Calendar
		/// </summary>
		private void btnCalendar_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Calendar();
			//window.Owner = this;
			//window.Show();
		}

		/// <summary>
		/// Mail
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMail_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Mail();
			//window.Owner = this;
			//window.Show();
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
		/// Admin - Community
		/// </summary>
		private void btnCommunity_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Forum();
			//window.Show();
		}

		/// <summary>
		/// Admin - Console SQL
		/// </summary>
		private void btnConsoleSQL_Click(object sender, RoutedEventArgs e)
		{
			var window = new ConsoleSQL();
			window.Show();
		}

		/// <summary>
		/// Articles - list
		/// </summary>
		private void btnArticlesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList();
			window.Show();
		}

		/// <summary>
		/// Articles - new
		/// </summary>
		private void btnArticlesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesNew(new C_Article(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Attachments - list
		/// </summary>
		private void btnAttachmentsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new AttachmentsList();
			window.Show();
		}

		/// <summary>
		/// Attachments - gallery
		/// </summary>
		private void btnAttachmentsGallery_Click(object sender, RoutedEventArgs e)
		{
			var window = new AttachmentsGallery();
			window.Show();
		}

		/// <summary>
		/// AttributesClasses - list
		/// </summary>
		private void btnAttributesClassesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new AttributesClassesList();
			window.Show();
		}

		/// <summary>
		/// AttributesClasses - new
		/// </summary>
		private void btnAttributesClassesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new AttributesClassesNew(new C_AttributeClass(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Companies - list
		/// </summary>
		private void btnCompaniesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesList();
			window.Show();
		}

		/// <summary>
		/// Companies - new
		/// </summary>
		private void btnCompaniesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesNew(new C_Company(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Distributions - list
		/// </summary>
		private void btnDistributionsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new DistributionsList();
			window.Show();
		}

		/// <summary>
		/// Distributions - new
		/// </summary>
		private void btnDistributionsNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new DistributionsNew(new C_Distribution(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Documents - list
		/// </summary>
		private void btnDocumentsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsList();
			window.Show();
		}

		/// <summary>
		/// Documents - new
		/// </summary>
		private void btnDocumentsNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsNew(new C_Document(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Employees - list
		/// </summary>
		private void btnEmployeesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesList();
			window.Show();
		}

		/// <summary>
		/// Employees - new
		/// </summary>
		private void btnEmployeesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesNew(new C_Employee(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Families - list
		/// </summary>
		private void btnFamiliesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesList();
			window.Show();
		}

		/// <summary>
		/// Families - new
		/// </summary>
		private void btnFamiliesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesNew(new C_Family(), Commands.Type.NEW);
			window.Show();
		}

		/// <summary>
		/// Groups
		/// </summary>
		private void btnGroupsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new GroupsList();
			window.Show();
		}

		/// <summary>
		/// Logs - list
		/// </summary>
		private void btnLogsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new LogsList();
			window.Show();
		}

		/// <summary>
		/// Logs - error list
		/// </summary>
		private void btnLogsErrorsList_Click(object sender, RoutedEventArgs e)
		{
			//var window = new ErrorsList();
			//window.Show();
		}

		/// <summary>
		/// Stats
		/// </summary>
		private void btnStats_Click(object sender, RoutedEventArgs e)
		{
			var window = new Stats.Stats();
			window.Show();
		}

		/// <summary>
		/// Stores - list
		/// </summary>
		private void btnStoresList_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList();
			window.Show();
		}

		/// <summary>
		/// Stores - new
		/// </summary>
		private void btnStoresNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList();
			window.Show();
		}

		/// <summary>
		/// Users - list
		/// </summary>
		private void btnUsersList_Click(object sender, RoutedEventArgs e)
		{
			//var window = new UsersList();
			//window.Show();
			Functions.OpenWindow_Module(this, Global.Module.USERS, Commands.Type.LIST);
		}

		/// <summary>
		/// Users - new
		/// </summary>
		private void btnUsersNew_Click(object sender, RoutedEventArgs e)
		{
			//var window = new UsersNew(new C_User(), Commands.Type.NEW);
			//window.Show();
			Functions.OpenWindow_Module(this, Global.Module.USERS, Commands.Type.NEW);
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
