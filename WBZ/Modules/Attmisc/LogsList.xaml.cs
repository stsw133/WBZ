using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Families;
using MODULE_CLASS = WBZ.Classes.C_Log;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for LogsList.xaml
	/// </summary>
	public partial class LogsList : Window
	{
		M_LogsList M = new M_LogsList();

		public LogsList()
		{
			InitializeComponent();
			DataContext = M;
			btnRefresh_Click(null, null);

			if (C_Config.Logs_Enabled == "1")
				chckEnabled.IsChecked = true;
			else
				chckEnabled.IsChecked = false;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			M.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{M.Filters.UserFullname.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.module,'')) like '%{M.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.content,'')) like '%{M.Filters.Content.ToLower()}%' and "
						+ $"l.datetime between '{M.Filters.fDateTime:yyyy-MM-dd}' and '{M.Filters.DateTime:yyyy-MM-dd} 23:59:59' and ";

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Apply filters
		/// </summary>
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Clear filters
		/// </summary>
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Preview
		/// </summary>
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
				openObj(instance, Global.ActionType.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
				openObj(instance, Global.ActionType.EDIT);
		}

		private void openObj(MODULE_CLASS log, Global.ActionType mode)
		{
			if (log.Instance == 0)
				return;
			if (SQL.CountInstances(log.Module, $"{string.Join("", log.Module.Split('_').AsQueryable().Cast<string>().Select(str => str.Substring(0, 1)))}.id={log.Instance}") == 0)
				return;

			if (!(mode == Global.ActionType.EDIT && Global.User.Perms.Contains($"{log.Module}_{Global.UserPermType.SAVE}")))
				mode = Global.ActionType.PREVIEW;
			if (!Global.User.Perms.Contains($"{log.Module}_{Global.UserPermType.PREVIEW}") && !Global.User.Perms.Contains($"{log.Module}_{Global.UserPermType.SAVE}"))
				return;
			Window window;

			switch (log.Module)
            {
				/// articles
				case Global.Module.ARTICLES:
					window = new ArticlesNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Article>()?[0], mode);
					break;
				/// attributes_classes
				case Global.Module.ATTRIBUTES_CLASSES:
					window = new AttributesClassesNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_AttributeClass>()?[0], mode);
					break;
				/// companies
				case Global.Module.COMPANIES:
					window = new CompaniesNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Company>()?[0], mode);
					break;
				/// distributions
				case Global.Module.DISTRIBUTIONS:
					window = new DistributionsNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Distribution>()?[0], mode);
					break;
				/// documents
				case Global.Module.DOCUMENTS:
					window = new DocumentsNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Document>()?[0], mode);
					break;
				/// families
				case Global.Module.FAMILIES:
					window = new FamiliesNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Family>()?[0], mode);
					break;
				/// stores
				case Global.Module.STORES:
					window = new StoresNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_Store>()?[0], mode);
					break;
				/// users
				case Global.Module.USERS:
					window = new UsersNew(SQL.GetInstance(log.Module, log.Instance).DataTableToList<C_User>()?[0], mode);
					break;
				default:
					return;
			}
			window.Show();
		}

		/// <summary>
		/// Delete
		/// </summary>
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (MODULE_CLASS instance in selectedInstances)
					SQL.DeleteInstance(M.MODULE_NAME, instance.ID, null);
				btnRefresh_Click(null, null);
			}
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(M.MODULE_NAME, M.FilterSQL);
				M.InstancesList = SQL.ListInstances(M.MODULE_NAME, M.FilterSQL, M.SORTING, M.Page = 0).DataTableToList<MODULE_CLASS>();

				foreach (var instance in M.InstancesList)
					instance.TranslatedModule = Global.TranslateModule(instance.Module);
			});
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Select
		/// </summary>
		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				btnEdit_Click(null, null);
		}

		/// <summary>
		/// Load more
		/// </summary>
		private void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && M.InstancesList.Count < M.TotalItems)
			{
				DataContext = null;
				M.InstancesList.AddRange(SQL.ListInstances(Global.Module.LOGS, M.FilterSQL, Properties.Settings.Default.sorting_LogsList, ++M.Page).DataTableToList<MODULE_CLASS>());
				for (int i = M.InstancesList.Count - 1; i >= Math.Max(M.InstancesList.Count - Convert.ToInt32(M.SORTING[4]), 0); i--)
					M.InstancesList[i].TranslatedModule = Global.TranslateModule(M.InstancesList[i].Module);
				DataContext = M;
				Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
			}
		}

		private void chckEnabled_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
				SQL.SetPropertyValue("LOGS_ENABLED", "1");
			else
				SQL.SetPropertyValue("LOGS_ENABLED", "0");
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_LogsList : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.LOGS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_LogsList;

		/// Logged user
		public C_User User { get; } = Global.User;
		/// Instance list
		private List<MODULE_CLASS> instancesList;
		public List<MODULE_CLASS> InstancesList
		{
			get
			{
				return instancesList;
			}
			set
			{
				instancesList = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Selecting mode
		public bool SelectingMode { get; set; }
		/// SQL filter
		public string FilterSQL { get; set; }
		/// Filter instance
		private MODULE_CLASS filters = new MODULE_CLASS();
		public MODULE_CLASS Filters
		{
			get
			{
				return filters;
			}
			set
			{
				filters = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Page number
		private int page;
		public int Page
		{
			get
			{
				return page;
			}
			set
			{
				page = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Total instances number
		private int totalItems;
		public int TotalItems
		{
			get
			{
				return totalItems;
			}
			set
			{
				totalItems = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}

		/// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
