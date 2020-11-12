using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;
using WBZ.Models;
using WBZ.Prototypes;
using MODULE_CLASS = WBZ.Models.C_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ItemsList.xaml
	/// </summary>
	public partial class ArticlesList : Window
	{
		D_ArticlesList D = new D_ArticlesList();

		public ArticlesList(bool selectingMode = false)
		{
			InitializeComponent();
			DataContext = D;
			D.SelectingMode = selectingMode;
		}

		/// <summary>
		/// Loaded
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			/*
			if (D.SelectingMode)
				dgList.SelectionMode = DataGridSelectionMode.Single;

			/// auto-select first store
			if (D.StoresList.Rows.Count > 0)
				cbStore.SelectedIndex = 0;
			*/
			P_ModuleList.Window_Loaded(this);
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			/*
			D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"a.archival=false and " : "")
						+ (SelectedStore?.ID > 0 ? $"sa.store={SelectedStore.ID} and " : "");

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
			*/
			P_ModuleList.UpdateFilters(this);
		}

		/// <summary>
		/// Apply filters
		/// </summary>
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			/*
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
			*/
			P_ModuleList.dpFilter_KeyUp(this, e);
		}

		/// <summary>
		/// Clear filters
		/// </summary>
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			D.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
			*/
			P_ModuleList.btnFiltersClear_Click(this);
		}

		/// <summary>
		/// Preview
		/// </summary>
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
			{
				var window = new ArticlesNew(instance, Commands.Type.PREVIEW);
				window.Show();
			}
			*/
			P_ModuleList.btnPreview_Click(this, GetType().FullName);
		}

		/// <summary>
		/// New
		/// </summary>
		private void btnNew_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var window = new ArticlesNew(new MODULE_CLASS(), Commands.Type.NEW);
			window.Show();
			*/
			P_ModuleList.btnNew_Click(this, GetType().FullName);
		}

		/// <summary>
		/// Duplicate
		/// </summary>
		private void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
			{
				var window = new ArticlesNew(instance, Commands.Type.DUPLICATE);
				window.Show();
			}
			*/
			P_ModuleList.btnDuplicate_Click(this, GetType().FullName);
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			foreach (MODULE_CLASS instance in selectedInstances)
			{
				var window = new ArticlesNew(instance, Commands.Type.EDIT);
				window.Show();
			}
			*/
			P_ModuleList.btnEdit_Click(this, GetType().FullName);
		}

		/// <summary>
		/// Delete
		/// </summary>
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
			if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (MODULE_CLASS instance in selectedInstances)
					SQL.DeleteInstance(D.MODULE_NAME, instance.ID, instance.Name);
				btnRefresh_Click(null, null);
			}
			*/
			P_ModuleList.btnDelete_Click(this);
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			await Task.Run(() => {
				UpdateFilters();
				D.TotalItems = SQL.CountInstances(D.MODULE_NAME, D.FilterSQL);
				D.InstancesList = SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, D.Page = 0).DataTableToList<MODULE_CLASS>();
			});
			*/
			P_ModuleList.btnRefresh_Click(this);
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			//Close();
			P_ModuleList.btnClose_Click(this);
		}

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		public C_Store SelectedStore;
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance(Global.Module.STORES, cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0).DataTableToList<C_Store>()?[0];
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Select
		/// </summary>
		public MODULE_CLASS Selected;
		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			/*
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!D.SelectingMode)
				{
					if (Global.User.Perms.Contains($"{D.MODULE_NAME}_{Global.UserPermType.SAVE}"))
						btnEdit_Click(null, null);
					else
						btnPreview_Click(null, null);
				}
				else
				{
					Selected = dgList.SelectedItems.Cast<MODULE_CLASS>().FirstOrDefault();
					DialogResult = true;
				}
			}
			*/
			P_ModuleList.dgList_MouseDoubleClick(this, e);
		}

		/// <summary>
		/// Load more
		/// </summary>
		private void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			/*
			if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
			{
				DataContext = null;
				D.InstancesList.AddRange(SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, ++D.Page).DataTableToList<MODULE_CLASS>());
				DataContext = D;
				Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
			}
			*/
			P_ModuleList.dgList_ScrollChanged(this, sender, e);
		}

		/// <summary>
		/// Closed
		/// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
			P_ModuleList.Window_Closed(this);
		}
    }
}
