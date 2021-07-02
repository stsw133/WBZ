using StswExpress;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
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
                WrpPanModules.Children.Remove(modVehicles);
            ///USERS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Users)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modUsers);
            ///STORES
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Stores)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modStores);
            ///LOGS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Logs)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modLogs);
            ///ICONS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Icons)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modIcons);
            ///FAMILIES
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Families)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modFamilies);
            ///EMPLOYEES
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Employees)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modEmployees);
            ///DOCUMENTS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Documents)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modDocuments);
            ///DISTRIBUTIONS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Distributions)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modDistributions);
            ///CONTRACTORS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Contractors)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modContractors);
            ///ATTRIBUTES_CLASSES
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(AttributesClasses)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modAttributesClasses);
            ///ATTACHMENTS
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Attachments)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modAttachments);
            ///ARTICLES
            if (!Config.User.Perms.Contains($"{Config.GetModule(nameof(Articles)).Name}_{Config.PermType.PREVIEW}"))
                WrpPanModules.Children.Remove(modArticles);
            ///ADMIN
            if (!Config.User.Perms.Contains($"{nameof(Admin)}"))
                WrpPanModules.Children.Remove(modAdmin);

            var coll = WrpPanModules.Children.Cast<Border>().OrderBy(x => (x.Child as IconButton).Text).ToList();
            WrpPanModules.Children.RemoveRange(0, WrpPanModules.Children.Count);
            foreach (var item in coll)
                WrpPanModules.Children.Add(item);
        }

        /// <summary>
        /// Opens context menu
        /// </summary>
        private void BtnContextMenu_Click(object sender, RoutedEventArgs e) => Fn.OpenContextMenu(sender);

        /// Menu
        internal void MnuItmRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Config.User = SQL.GetInstance<M_User>(Config.GetModule(nameof(Users)), Config.User.ID);
            Config.User.Perms = SQL.GetUserPerms(Config.User.ID);

            new Main().Show();

            D.WantToLogout = true;
            Close();
        }
        private void MnuItmLogout_Click(object sender, RoutedEventArgs e)
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
        private void MnuItmClose_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        /// Personal
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => new Profile() { Owner = this }.ShowDialog();
        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            //new Calendar() { Owner = this }.ShowDialog();
        }
        private void BtnMail_Click(object sender, RoutedEventArgs e)
        {
            //new Mail() { Owner = this }.ShowDialog();
        }

        /// <summary>
        /// Search - TextChanged
        /// </summary>
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox).Text.ToLower();
            foreach (Border module in WrpPanModules.Children)
            {
                var btn = module.Child as IconButton;
                var moduleName = btn.Text.ToLower();
                module.Visibility = moduleName.Contains(searchText.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// Admin
        private void BtnConsoleSQL_Click(object sender, RoutedEventArgs e) => new ConsoleSQL() { Owner = this }.Show();
        private void BtnCommunity_Click(object sender, RoutedEventArgs e)
        {
            //new Forum() { Owner = this }.ShowDialog();
        }

        /// Articles
        private void BtnArticlesList_Click(object sender, RoutedEventArgs e) => new ArticlesList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnArticlesNew_Click(object sender, RoutedEventArgs e) => new ArticlesNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Attachments
        private void BtnAttachmentsList_Click(object sender, RoutedEventArgs e) => new AttachmentsList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnAttachmentsGallery_Click(object sender, RoutedEventArgs e) => new AttachmentsGallery() { Owner = this }.Show();

        /// AttributesClasses
        private void BtnAttributesClassesList_Click(object sender, RoutedEventArgs e) => new AttributesClassesList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnAttributesClassesNew_Click(object sender, RoutedEventArgs e) => new AttributesClassesNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Contractors
        private void BtnContractorsList_Click(object sender, RoutedEventArgs e) => new ContractorsList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnContractorsNew_Click(object sender, RoutedEventArgs e) => new ContractorsNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Distributions
        private void BtnDistributionsList_Click(object sender, RoutedEventArgs e) => new DistributionsList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnDistributionsNew_Click(object sender, RoutedEventArgs e) => new DistributionsNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Documents
        private void BtnDocumentsList_Click(object sender, RoutedEventArgs e) => new DocumentsList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnDocumentsNew_Click(object sender, RoutedEventArgs e) => new DocumentsNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Employees
        private void BtnEmployeesList_Click(object sender, RoutedEventArgs e) => new EmployeesList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnEmployeesNew_Click(object sender, RoutedEventArgs e) => new EmployeesNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Families
        private void BtnFamiliesList_Click(object sender, RoutedEventArgs e) => new FamiliesList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnFamiliesNew_Click(object sender, RoutedEventArgs e) => new FamiliesNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Icons
        private void BtnIconsList_Click(object sender, RoutedEventArgs e) => new IconsList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnIconsNew_Click(object sender, RoutedEventArgs e) => new IconsNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Logs
        private void BtnLogsList_Click(object sender, RoutedEventArgs e) => new LogsList(Commands.Type.LIST) { Owner = this }.Show();

        /// Stores
        private void BtnStoresList_Click(object sender, RoutedEventArgs e) => new StoresList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnStoresNew_Click(object sender, RoutedEventArgs e) => new StoresNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Users
        private void BtnUsersList_Click(object sender, RoutedEventArgs e) => new UsersList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnUsersNew_Click(object sender, RoutedEventArgs e) => new UsersNew(null, Commands.Type.NEW) { Owner = this }.Show();

        /// Vehicles
        private void BtnVehiclesList_Click(object sender, RoutedEventArgs e) => new VehiclesList(Commands.Type.LIST) { Owner = this }.Show();
        private void BtnVehiclesNew_Click(object sender, RoutedEventArgs e) => new VehiclesNew(null, Commands.Type.NEW) { Owner = this }.Show();

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
