using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;

namespace WBZ.Prototypes
{
    internal class P_ModuleList<MODULE_MODEL>
    {
        dynamic W, D;
        string Fullname;
        string MODULE_NAME;

        internal P_ModuleList(object owner)
        {
            W = owner;
            D = W.DataContext;
            Fullname = owner.GetType().FullName;
            MODULE_NAME = D.MODULE_NAME;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        internal void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D.SelectingMode)
                W.dgList.SelectionMode = DataGridSelectionMode.Single;

            switch (MODULE_NAME)
            {
                ///ARTICLES
                case Global.Module.ARTICLES:
                    /// auto-select first store
                    if (D.StoresList.Rows.Count > 0)
                        W.cbStore.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
		/// Update filters
		/// </summary>
		internal void UpdateFilters()
        {
            switch (MODULE_NAME)
            {
                ///ARTICLES
                case Global.Module.ARTICLES:
                    D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"a.archival=false and " : "")
                        + (W.SelectedStore?.ID > 0 ? $"sa.store={W.SelectedStore.ID} and " : "");
                    break;
            }

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }

        /// <summary>
		/// Apply filters
		/// </summary>
		internal void dpFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnRefresh_Click(null, null);
        }

        /// <summary>
		/// Clear filters
		/// </summary>
		internal void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
        {
            D.Filters = new MODULE_MODEL();
            btnRefresh_Click(null, null);
        }

        /// <summary>
        /// Preview
        /// </summary>
        internal void btnPreview_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(Fullname), instance, Commands.Type.PREVIEW) as Window;
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		internal void btnNew_Click(object sender, MouseButtonEventArgs e)
        {
            var window = Activator.CreateInstance(Type.GetType(Fullname), null, Commands.Type.NEW) as Window;
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        internal void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(Fullname), instance, Commands.Type.DUPLICATE) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        internal void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(Fullname), instance, Commands.Type.EDIT) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        internal void btnDelete_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (dynamic instance in selectedInstances)
                    SQL.DeleteInstance(D.MODULE_NAME, instance.ID, instance.Name);
                btnRefresh_Click(null, null);
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        internal async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            await Task.Run(() => {
                UpdateFilters();
                D.TotalItems = SQL.CountInstances(D.MODULE_NAME, D.FilterSQL);
                D.InstancesList = SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, D.Page = 0).DataTableToList<MODULE_MODEL>();
            });
        }

        /// <summary>
        /// Close
        /// </summary>
        internal void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            W.Close();
        }

        /// <summary>
        /// Select
        /// </summary>
        internal void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
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
                    W.Selected = W.dgList.SelectedItems.Cast<MODULE_MODEL>().FirstOrDefault();
                    W.DialogResult = true;
                }
            }
        }

        /// <summary>
        /// Load more
        /// </summary>
        internal void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                W.DataContext = null;
                D.InstancesList.AddRange(SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING, ++D.Page).DataTableToList<MODULE_MODEL>());
                W.DataContext = D;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        internal void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
