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
			if (!Global.User.Perms.Contains($"{Global.Module.STATS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.RemoveAt(7);

			if ((!Global.User.Perms.Contains($"{Global.Module.ATTRIBUTES_CLASSES}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.ATTACHMENTS}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.LOGS}_{Global.UserPermType.PREVIEW}")))
				gridModules.Children.RemoveAt(6);

			if (!Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.RemoveAt(5);

			if (!Global.User.Perms.Contains($"{Global.Module.FAMILIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.RemoveAt(4);

			if (!Global.User.Perms.Contains($"{Global.Module.COMPANIES}_{Global.UserPermType.PREVIEW}"))
				gridModules.Children.RemoveAt(3);

			if ((!Global.User.Perms.Contains($"{Global.Module.ARTICLES}_{Global.UserPermType.PREVIEW}"))
			 && (!Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.PREVIEW}")))
				gridModules.Children.RemoveAt(2);

			if (!Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.PREVIEW}"))
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
		}

		/// <summary>
		/// Admin - UsersList
		/// </summary>
		private void btnUsersList_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList();
			window.Show();
		}

		/// <summary>
		/// Admin - EmployeesList
		/// </summary>
		private void btnEmployeesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesList();
			window.Show();
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
#if DEBUG
			var window = new ConsoleSQL();
			window.Show();
#endif
		}

		/// <summary>
		/// Documents - DocumentsNew
		/// </summary>
		private void btnDocumentsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsList();
			window.Show();
		}

		/// <summary>
		/// Documents - DocumentsNew
		/// </summary>
		private void btnDocumentsNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new DocumentsNew(new C_Document(), Global.ActionType.NEW);
			window.Show();
		}

		/// <summary>
		/// Articles - ArticlesList
		/// </summary>
		private void btnArticlesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList();
			window.Show();
		}

		/// <summary>
		/// Articles - ArticlesNew
		/// </summary>
		private void btnArticlesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesNew(new C_Article(), Global.ActionType.NEW);
			window.Show();
		}

		/// <summary>
		/// Articles - StoresList
		/// </summary>
		private void btnStoresList_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList();
			window.Show();
		}

		/// <summary>
		/// Companies - CompaniesList
		/// </summary>
		private void btnCompaniesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesList();
			window.Show();
		}

		/// <summary>
		/// Companies - CompaniesNew
		/// </summary>
		private void btnCompaniesNew_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesNew(new C_Company(), Global.ActionType.NEW);
			window.Show();
		}

		/// <summary>
		/// Families - FamiliesList
		/// </summary>
		private void btnFamiliesList_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesList();
			window.Show();
		}

		/// <summary>
		/// Families - FamiliesNew
		/// </summary>
		private void btnFamiliesAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new FamiliesNew(new C_Family(), Global.ActionType.NEW);
			window.Show();
		}

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
		private void btnGroupsList_Click(object sender, RoutedEventArgs e)
		{
			var window = new GroupsList();
			window.Show();
		}
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
