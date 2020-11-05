using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Controls;
using WBZ.Helpers;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Attmisc;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Login;
using WBZ.Modules.Personal;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;
using WBZ.Modules.Groups;
using WBZ.Modules.AttributesClasses;

namespace WBZ.Modules
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		readonly M_Main M = new M_Main();

		public Main()
		{
			InitializeComponent();
			DataContext = M;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			///ATTMISC
			if ((!Global.User.Perms.Contains($"{Global.Module.GROUPS}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.ATTRIBUTES_CLASSES}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.ATTACHMENTS}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.LOGS}_{Global.UserPermType.PREVIEW}")))
				gridModules.Children.Remove(modAttmisc);
			///USERS
			if (!Global.User.Perms.Contains($"{Global.Module.USERS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modUsers);
			///EMPLOYEES
			if (!Global.User.Perms.Contains($"{Global.Module.EMPLOYEES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modEmployees);
			///STATS
			if (!Global.User.Perms.Contains($"{Global.Module.STATS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modStats);
			///DISTRIBUTIONS
			if (!Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modDistributions);
			///FAMILIES
			if (!Global.User.Perms.Contains($"{Global.Module.FAMILIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modFamilies);
			///COMPANIES
			if (!Global.User.Perms.Contains($"{Global.Module.COMPANIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modCompanies);
			///STORES
			if (!Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modStores);
			///ARTICLES
			if (!Global.User.Perms.Contains($"{Global.Module.ARTICLES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modArticles);
			///DOCUMENTS
			if (!Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.Remove(modDocuments);
			///ADMIN
			if (!Global.User.Perms.Contains($"admin"))
				gridModules.Children.Remove(modAdmin);
		}

		/// <summary>
		/// Menu - Refresh
		/// </summary>
		private void menuRefresh_Click(object sender, RoutedEventArgs e)
		{
			Global.User = SQL.GetInstance(Global.Module.USERS, Global.User.ID).DataTableToList<C_User>()?[0];
			Global.User.Perms = SQL.GetUserPerms(Global.User.ID);

			var window = new Main();
			window.Show();

			M.WantToLogout = true;
			Close();
		}

		/// <summary>
		/// Menu - Settings
		/// </summary>
		private void menuSettings_Click(object sender, RoutedEventArgs e)
		{
			var window = new Settings();
			window.ShowDialog();
		}

		/// <summary>
		/// Menu - Logout
		/// </summary>
		private void menuLogout_Click(object sender, RoutedEventArgs e)
		{
			if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.CONFIRMATION, "Na pewno dokonać wylogowania?") { Owner = this }.ShowDialog() == true)
			{
				Global.User = new C_User();

				foreach (Window x in App.Current.Windows)
					if (x != this)
						x.Close();

				var window = new Login.Login();
				window.Show();

				M.WantToLogout = true;
				Close();
			}
		}

		/// <summary>
		/// Menu - Close
		/// </summary>
		private void menuClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Menu - Manual
		/// </summary>
		private void menuManual_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				string path = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/pl_manual.pdf";
				process.StartInfo.FileName = new Uri(path, UriKind.RelativeOrAbsolute).LocalPath;
				process.Start();
			}
			catch (Exception ex)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd otwierania poradnika: " + ex.Message) { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// Menu - AboutApp
		/// </summary>
		private void menuAboutApp_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginAppAbout();
			window.Owner = this;
			window.ShowDialog();
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
			var window = new DocumentsNew(new C_Document(), Global.ActionType.NEW);
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
			var window = new ArticlesNew(new C_Article(), Global.ActionType.NEW);
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
			var window = new CompaniesNew(new C_Company(), Global.ActionType.NEW);
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
			var window = new FamiliesNew(new C_Family(), Global.ActionType.NEW);
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
			var window = new DistributionsNew(new C_Distribution(), Global.ActionType.NEW);
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
		/// Stats
		/// </summary>
		private void btnStats_Click(object sender, RoutedEventArgs e)
		{
			var window = new Stats.Stats();
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
			var window = new EmployeesNew(new C_Employee(), Global.ActionType.NEW);
			window.Show();
		}

		/// <summary>
		/// Users - list
		/// </summary>
		private void btnUsersList_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList();
			window.Show();
		}

		/// <summary>
		/// Users - new
		/// </summary>
		private void btnUsersNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersNew(new C_User(), Global.ActionType.NEW);
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
			var window = new AttributesClassesNew(new C_AttributeClass(), Global.ActionType.NEW);
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
		/// Logs - list
		/// </summary>
		private void btnLogsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new LogsList();
			window.Show();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
        {
			Properties.Settings.Default.Save();
            if (!M.WantToLogout)
                foreach (Window x in App.Current.Windows)
                    if (x != this)
                        x.Close();
        }
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_Main : INotifyPropertyChanged
	{
		/// PropertyList
		public C_User User { get; } = Global.User;
		public string Title { get; } = Global.Database.Name + " - okno główne";
		public bool WantToLogout { get; set; } = false;
		
		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
