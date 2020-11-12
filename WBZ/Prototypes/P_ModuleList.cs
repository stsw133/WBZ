using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;

namespace WBZ.Prototypes
{
    internal static class P_ModuleList
    {
        /// <summary>
        /// Loaded
        /// </summary>
        internal static void Window_Loaded(dynamic owner)
        {
            var D = owner.DataContext;

            if (D.SelectingMode)
                owner.dgList.SelectionMode = DataGridSelectionMode.Single;

            string moduleName = D.MODULE_NAME;
            switch (moduleName)
            {
                ///ARTICLES
                case Global.Module.ARTICLES:
                    /// auto-select first store
                    if (D.StoresList.Rows.Count > 0)
                        owner.cbStore.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
		/// Update filters
		/// </summary>
		internal static void UpdateFilters(dynamic owner)
        {
            var D = owner.DataContext;

            string moduleName = D.MODULE_NAME;
            switch (moduleName)
            {
                ///ARTICLES
                case Global.Module.ARTICLES:
                    D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"a.archival=false and " : "")
                        + (owner.SelectedStore?.ID > 0 ? $"sa.store={owner.SelectedStore.ID} and " : "");
                    break;
            }

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }

        /// <summary>
		/// Apply filters
		/// </summary>
		internal static void dpFilter_KeyUp(dynamic owner, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnRefresh_Click(owner);
        }

        /// <summary>
		/// Clear filters
		/// </summary>
		internal static void btnFiltersClear_Click(dynamic owner)
        {
            var D = owner.DataContext;

            D.Filters = new MODULE_CLASS();
            btnRefresh_Click(owner);
        }

        /// <summary>
        /// Preview
        /// </summary>
        internal static void btnPreview_Click(dynamic owner, string fullname)
        {
            var selectedInstances = owner.dgList.SelectedItems.Cast<MODULE_CLASS>();
            foreach (MODULE_CLASS instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(fullname), instance, Commands.Type.PREVIEW) as Window;
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		internal static void btnNew_Click(dynamic owner, string fullname)
        {
            var window = Activator.CreateInstance(Type.GetType(fullname), new MODULE_CLASS(), Commands.Type.NEW) as Window;
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        internal static void btnDuplicate_Click(dynamic owner, string fullname)
        {
            var selectedInstances = owner.dgList.SelectedItems.Cast<MODULE_CLASS>();
            foreach (MODULE_CLASS instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(fullname), new MODULE_CLASS(), Commands.Type.DUPLICATE) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        internal static void btnEdit_Click(dynamic owner, string fullname)
        {
            var selectedInstances = owner.dgList.SelectedItems.Cast<MODULE_CLASS>();
            foreach (MODULE_CLASS instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(fullname), instance, Commands.Type.EDIT) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        internal static void btnDelete_Click(dynamic owner)
        {
            var D = owner.DataContext;

            var selectedInstances = owner.dgList.SelectedItems.Cast<MODULE_CLASS>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (MODULE_CLASS instance in selectedInstances)
                    SQL.DeleteInstance(D.MODULE_NAME, instance.ID, instance.Name);
                btnRefresh_Click(owner);
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        internal static async void btnRefresh_Click(dynamic owner)
        {
            var D = owner.DataContext;

            await Task.Run(() => {
                UpdateFilters(owner);
                D.TotalItems = SQL.CountInstances(D.MODULE_NAME, D.FilterSQL);
                D.InstancesList = SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, D.Page = 0).DataTableToList<MODULE_CLASS>();
            });
        }

        /// <summary>
        /// Close
        /// </summary>
        internal static void btnClose_Click(dynamic owner)
        {
            owner.Close();
        }

        /// <summary>
        /// Select
        /// </summary>
        internal static void dgList_MouseDoubleClick(dynamic owner, MouseButtonEventArgs e)
        {
            var D = owner.DataContext;

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
                    owner.Selected = owner.dgList.SelectedItems.Cast<MODULE_CLASS>().FirstOrDefault();
                    owner.DialogResult = true;
                }
            }
        }

        /// <summary>
        /// Load more
        /// </summary>
        internal static void dgList_ScrollChanged(dynamic owner, object sender, ScrollChangedEventArgs e)
        {
            var D = owner.DataContext;

            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                DataContext = null;
                D.InstancesList.AddRange(SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, ++D.Page).DataTableToList<MODULE_CLASS>());
                DataContext = D;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        internal static void Window_Closed(dynamic owner)
        {
        }
    }
}
