using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Families;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Login;
using WBZ.Modules.Settings;
using WBZ.Modules.Stats;
using WBZ.Modules.Stores;
using WBZ.Modules.Attmisc;

namespace WBZ
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
			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.COMMUNITY}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(8);

			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.STATS}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(7);

			if ((!Global.User.Perms.Contains($"{Global.ModuleTypes.ATTRIBUTES_CLASSES}_{Global.UserPermTypes.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.ModuleTypes.ATTACHMENTS}_{Global.UserPermTypes.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.ModuleTypes.LOGS}_{Global.UserPermTypes.PREVIEW}")))
				gridModules.Children.RemoveAt(6);

			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.DISTRIBUTIONS}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(5);

			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.FAMILIES}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(4);

			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.COMPANIES}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(3);

			if ((!Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.ModuleTypes.STORES}_{Global.UserPermTypes.PREVIEW}")))
				gridModules.Children.RemoveAt(2);

			if (!Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.PREVIEW}"))
				gridModules.Children.RemoveAt(1);

			if (!Global.User.Perms.Contains($"admin"))
				gridModules.Children.RemoveAt(0);
		}

		#region menu
		private void menuSettings_Click(object sender, RoutedEventArgs e)
		{
			var window = new AppSettings();
			window.ShowDialog();
		}
		private void menuLogout_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Czy na pewno się wylogować?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Global.User = new C_User();

				foreach (Window x in App.Current.Windows)
					if (x != this)
						x.Close();

				var window = new Login();
				window.Show();

				M.WantToLogout = true;
				Close();
			}
		}
		private void menuClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
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
				MessageBox.Show(ex.Message);
			}
		}
		private void menuAboutApp_Click(object sender, RoutedEventArgs e)
		{
			var window = new LoginAppAbout();
			window.Owner = this;
			window.ShowDialog();
		}
		#endregion

		#region profile
		private void btnOther_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}
		private void btnProfile_Click(object sender, RoutedEventArgs e)
		{
			var window = new ProfileSettings();
			window.ShowDialog();
		}
		private void btnCalendar_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Calendar();
			//window.Owner = this;
			//window.Show();
		}
		private void btnMail_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Mail();
			//window.Owner = this;
			//window.Show();
		}
		#endregion

		private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			var searchText = (sender as TextBox).Text.ToLower();

			foreach (DockPanel module in gridModules.Children)
			{
				var expHeader = (module.Children[1] as Expander).Header.ToString().ToLower();

				if (expHeader.Contains(searchText))
					module.Visibility = Visibility.Visible;
				else
					module.Visibility = Visibility.Collapsed;
			}
		}
		
		private void expModule_Expanded(object sender, RoutedEventArgs e)
		{
			if (!sender.Equals(expAdmin))
				expAdmin.IsExpanded = false;
			if (!sender.Equals(expDocuments))
				expDocuments.IsExpanded = false;
			if (!sender.Equals(expArticles))
				expArticles.IsExpanded = false;
			if (!sender.Equals(expCompanies))
				expCompanies.IsExpanded = false;
			if (!sender.Equals(expFamilies))
				expFamilies.IsExpanded = false;
			if (!sender.Equals(expDistributions))
				expDistributions.IsExpanded = false;
			if (!sender.Equals(expAttmisc))
				expAttmisc.IsExpanded = false;
			if (!sender.Equals(expStats))
				expStats.IsExpanded = false;
			if (!sender.Equals(expCommunity))
				expCommunity.IsExpanded = false;
		}

		#region module "Admin"
		private void btnUsersList_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList();
			window.Show();
		}
		private void btnEmployeesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesList();
			window.Show();
		}
		#endregion

		#region module "Documents"
		private void btnDocumentsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsList();
			window.Show();
		}
		private void btnDocumentsAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsAdd(new C_Document(), true);
			window.Show();
		}
		#endregion

		#region module "Articles"
		private void btnArticlesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList();
			window.Show();
		}
		private void btnArticlesAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesAdd(new C_Article(), true);
			window.Show();
		}
		private void btnStoresList_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList();
			window.Show();
		}
		#endregion

		#region module "Companies"
		private void btnCompaniesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesList();
			window.Show();
		}
		private void btnCompaniesAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesAdd(new C_Company(), true);
			window.Show();
		}
		#endregion

		#region module "Families"
		private void btnFamiliesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesList();
			window.Show();
		}
		private void btnFamiliesAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesAdd(new C_Family(), true);
			window.Show();
		}
		#endregion

		#region module "Distributions"
		private void btnDistributionsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new DistributionsList();
			window.Show();
		}
		private void btnDistributionsAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new DistributionsAdd(new C_Distribution(), true);
			window.Show();
		}
		#endregion

		#region module "Attmisc"
		private void btnAttributesClassesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new AttributesClassesList();
			window.Show();
		}
		private void btnAttachmentsList_Click(object sender, RoutedEventArgs e)
		{
			//var window = new AttachmentsList();
			//window.Show();
		}
		private void btnAttachmentsGallery_Click(object sender, RoutedEventArgs e)
		{
			//var window = new AttachmentsGallery();
			//window.Show();
		}
		private void btnLogsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new LogsList();
			window.Show();
		}
		#endregion

		#region module "Stats"
		private void btnStats_Click(object sender, RoutedEventArgs e)
		{
			var window = new Stats();
			window.Show();
		}
		#endregion

		#region module "Community"
		private void btnForum_Click(object sender, RoutedEventArgs e)
		{
			//var window = new Forum();
			//window.Show();
		}
		#endregion

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
