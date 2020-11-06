using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Families;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;
using MODULE_CLASS = WBZ.Models.C_Log;

namespace WBZ.Modules.Logs
{
	/// <summary>
	/// Interaction logic for LogsList.xaml
	/// </summary>
	public partial class LogsList : Window
	{
		D_LogsList D = new D_LogsList();

		public LogsList()
		{
			InitializeComponent();
			DataContext = D;
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
			D.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{D.Filters.UserFullname.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.content,'')) like '%{D.Filters.Content.ToLower()}%' and "
						+ $"l.datetime between '{D.Filters.fDateTime:yyyy-MM-dd}' and '{D.Filters.DateTime:yyyy-MM-dd} 23:59:59' and ";

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
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
			D.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Preview
		/// </summary>
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
				openObj(instance, Commands.Type.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
				openObj(instance, Commands.Type.EDIT);
		}

		private void openObj(MODULE_CLASS log, Commands.Type mode)
		{
			if (log.Instance == 0)
				return;
			if (SQL.CountInstances(log.Module, $"{string.Join("", log.Module.Split('_').AsQueryable().Cast<string>().Select(str => str.Substring(0, 1)))}.id={log.Instance}") == 0)
				return;

			if (!(mode == Commands.Type.EDIT && Global.User.Perms.Contains($"{log.Module}_{Global.UserPermType.SAVE}")))
				mode = Commands.Type.PREVIEW;
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
					SQL.DeleteInstance(D.MODULE_NAME, instance.ID, null);
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
				D.TotalItems = SQL.CountInstances(D.MODULE_NAME, D.FilterSQL);
				D.InstancesList = SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, D.Page = 0).DataTableToList<MODULE_CLASS>();
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
			if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
			{
				DataContext = null;
				D.InstancesList.AddRange(SQL.ListInstances(Global.Module.LOGS, D.FilterSQL, D.SORTING, ++D.Page).DataTableToList<MODULE_CLASS>());
				DataContext = D;
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
}
