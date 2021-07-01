using StswExpress;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Globals;
using WBZ.Modules._base;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Attachments;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Contractors;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Icons;
using WBZ.Modules.Logs;
using WBZ.Modules.Personal;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;
using WBZ.Modules.Vehicles;

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
			///VEHICLES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Vehicles)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modVehicles);
			///USERS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Users)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modUsers);
			///STORES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Stores)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modStores);
			///STATS
			//if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Stats)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modStats);
			///LOGS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Logs)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modLogs);
			///ICONS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Icons)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modIcons);
			///FAMILIES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Families)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modFamilies);
			///EMPLOYEES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Employees)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modEmployees);
			///DOCUMENTS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Documents)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modDocuments);
			///DISTRIBUTIONS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Distributions)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modDistributions);
			///CONTRACTORS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Contractors)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modContractors);
			///ATTRIBUTES_CLASSES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(AttributesClasses)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modAttributesClasses);
			///ATTACHMENTS
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Attachments)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modAttachments);
			///ARTICLES
			if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Articles)).Name}_{Config.PermType.PREVIEW}"))
				gridModules.Children.Remove(modArticles);
			///ADMIN
			if (!Config.User.Perms.Contains($"Admin"))
				gridModules.Children.Remove(modAdmin);

			var coll = gridModules.Children.Cast<Border>().OrderBy(x => (x.Child as IconButton).Text).ToList();
			gridModules.Children.RemoveRange(0, gridModules.Children.Count);
			foreach (var item in coll)
				gridModules.Children.Add(item);
		}

		/// <summary>
		/// Opens context menu
		/// </summary>
		private void btnContextMenu_Click(object sender, RoutedEventArgs e) => Fn.OpenContextMenu(sender);

		/// <summary>
		/// Menu
		/// </summary>
		internal void menuRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
            Config.User = SQL.GetInstance<M_User>(Config.GetModule(nameof(Users)), Config.User.ID);
			Config.User.Perms = SQL.GetUserPerms(Config.User.ID);

			new Main().Show();

			D.WantToLogout = true;
			Close();
		}
		private void menuLogout_Click(object sender, RoutedEventArgs e)
		{
			if (new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.QUESTION, "Na pewno wylogować?") { Owner = this }.ShowDialog() == true)
			{
				Config.User = new M_User();

				foreach (Window x in App.Current.Windows)
				{
					if (x != this)
						x.Close();
				}

				new Login.Login().Show();

				D.WantToLogout = true;
				Close();
			}
		}
		private void menuClose_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

		/// <summary>
		/// Personal
		/// </summary>
		private void btnProfile_Click(object sender, RoutedEventArgs e) => new Profile() { Owner = this }.ShowDialog();
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
				var btn = module.Child as IconButton;
				var moduleName = btn.Text.ToLower();

				module.Visibility = moduleName.Contains(searchText.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		/// <summary>
		/// Admin
		/// </summary>
		private void btnConsoleSQL_Click(object sender, RoutedEventArgs e) => new ConsoleSQL() { Owner = this }.Show();
		private void btnCommunity_Click(object sender, RoutedEventArgs e)
		{
			//new Forum() { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Articles
		/// </summary>
		private void btnArticlesList_Click(object sender, RoutedEventArgs e) => new ArticlesList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnArticlesNew_Click(object sender, RoutedEventArgs e) => new ArticlesNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Attachments
		/// </summary>
		private void btnAttachmentsList_Click(object sender, RoutedEventArgs e) => new AttachmentsList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnAttachmentsGallery_Click(object sender, RoutedEventArgs e) => new AttachmentsGallery() { Owner = this }.Show();

		/// <summary>
		/// AttributesClasses
		/// </summary>
		private void btnAttributesClassesList_Click(object sender, RoutedEventArgs e) => new AttributesClassesList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnAttributesClassesNew_Click(object sender, RoutedEventArgs e) => new AttributesClassesNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Contractors
		/// </summary>
		private void btnContractorsList_Click(object sender, RoutedEventArgs e) => new ContractorsList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnContractorsNew_Click(object sender, RoutedEventArgs e) => new ContractorsNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Distributions
		/// </summary>
		private void btnDistributionsList_Click(object sender, RoutedEventArgs e) => new DistributionsList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnDistributionsNew_Click(object sender, RoutedEventArgs e) => new DistributionsNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Documents
		/// </summary>
		private void btnDocumentsList_Click(object sender, RoutedEventArgs e) => new DocumentsList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnDocumentsNew_Click(object sender, RoutedEventArgs e) => new DocumentsNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Employees
		/// </summary>
		private void btnEmployeesList_Click(object sender, RoutedEventArgs e) => new EmployeesList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnEmployeesNew_Click(object sender, RoutedEventArgs e) => new EmployeesNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Families
		/// </summary>
		private void btnFamiliesList_Click(object sender, RoutedEventArgs e) => new FamiliesList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnFamiliesNew_Click(object sender, RoutedEventArgs e) => new FamiliesNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Icons
		/// </summary>
		private void btnIconsList_Click(object sender, RoutedEventArgs e) => new IconsList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnIconsNew_Click(object sender, RoutedEventArgs e) => new IconsNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Logs
		/// </summary>
		private void btnLogsList_Click(object sender, RoutedEventArgs e) => new LogsList(Commands.Type.LIST) { Owner = this }.Show();

		/// <summary>
		/// Stats
		/// </summary>
		private void btnStats_Click(object sender, RoutedEventArgs e) => new Stats.Stats() { Owner = this }.Show();

		/// <summary>
		/// Stores
		/// </summary>
		private void btnStoresList_Click(object sender, RoutedEventArgs e) => new StoresList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnStoresNew_Click(object sender, RoutedEventArgs e) => new StoresNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Users
		/// </summary>
		private void btnUsersList_Click(object sender, RoutedEventArgs e) => new UsersList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnUsersNew_Click(object sender, RoutedEventArgs e) => new UsersNew(null, Commands.Type.NEW) { Owner = this }.Show();

		/// <summary>
		/// Vehicles
		/// </summary>
		private void btnVehiclesList_Click(object sender, RoutedEventArgs e) => new VehiclesList(Commands.Type.LIST) { Owner = this }.Show();
		private void btnVehiclesNew_Click(object sender, RoutedEventArgs e) => new VehiclesNew(null, Commands.Type.NEW) { Owner = this }.Show();

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
