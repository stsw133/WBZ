using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;

namespace WBZ.Interfaces
{
    interface IModuleList
	{
        void Window_Loaded(object sender, RoutedEventArgs e);
        void UpdateFilters();
        void dpFilter_KeyUp(object sender, KeyEventArgs e);
        void btnFiltersClear_Click(object sender, MouseButtonEventArgs e);
        void btnPreview_Click(object sender, MouseButtonEventArgs e);
        void btnNew_Click(object sender, MouseButtonEventArgs e);
        void btnDuplicate_Click(object sender, MouseButtonEventArgs e);
        void btnEdit_Click(object sender, MouseButtonEventArgs e);
        void btnDelete_Click(object sender, MouseButtonEventArgs e);
        void btnRefresh_Click(object sender, MouseButtonEventArgs e);
        void btnClose_Click(object sender, MouseButtonEventArgs e);
        void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e);
        void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e);
        void Window_Closed(object sender, EventArgs e);
    }

    public class ModuleList<MODULE_MODEL> : Window, IModuleList where MODULE_MODEL : class, new()
    {
        dynamic W, D;
        string FullName, HalfName;
        string MODULE_TYPE;

        /// <summary>
        /// Loaded
        /// </summary>
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            W = GetWindow(this);
            D = W.DataContext;
            FullName = W.GetType().FullName;
            HalfName = FullName.Substring(0, FullName.Length - 4);
            MODULE_TYPE = D.MODULE_TYPE;

            if (D.SelectingMode)
                W.dgList.SelectionMode = DataGridSelectionMode.Single;
        }

        /// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
        {
            switch (MODULE_TYPE)
            {
                /// ARTICLES
                case Global.Module.ARTICLES:
                    D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"a.archival=false and " : "")
                        + (W.SelectedStore?.ID > 0 ? $"sa.store={W.SelectedStore.ID} and " : "");
                    break;
                /// ATTRIBUTES_CLASSES
                case Global.Module.ATTRIBUTES_CLASSES:
                    D.FilterSQL = $"LOWER(COALESCE(ac.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.type,'')) like '%{D.Filters.Type.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.values,'')) like '%{D.Filters.Values.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"ac.archival=false and " : "");
                    break;
                /// COMPANIES
                case Global.Module.COMPANIES:
                    D.FilterSQL = $"LOWER(COALESCE(c.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.branch,'')) like '%{D.Filters.Branch.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.nip,'')) like '%{D.Filters.NIP.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.regon,'')) like '%{D.Filters.REGON.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"c.archival=false and " : "");
                    break;
                /// EMPLOYEES
                case Global.Module.EMPLOYEES:
                    D.FilterSQL = $"LOWER(COALESCE(e.forename,'')) like '%{D.Filters.Forename.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.department,'')) like '%{D.Filters.Department.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.position,'')) like '%{D.Filters.Position.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.email,'')) like '%{D.Filters.Email.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.phone,'')) like '%{D.Filters.Phone.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (D.Filters.Archival ? $"e.archival=false and " : "");
                    break;
                /// STORES
                case Global.Module.STORES:
                    D.FilterSQL = $"LOWER(COALESCE(s.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"s.archival=false and " : "");
                    break;
                /// USERS
                case Global.Module.USERS:
                    D.FilterSQL = $"LOWER(COALESCE(u.forename,'')) like '%{D.Filters.Forename.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.email,'')) like '%{D.Filters.Email.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.phone,'')) like '%{D.Filters.Phone.ToLower()}%' and "
                        + (D.Filters.Archival ? $"u.archival=false and " : "");
                    break;
            }

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }

        /// <summary>
		/// Apply filters
		/// </summary>
		public void dpFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnRefresh_Click(null, null);
        }

        /// <summary>
		/// Clear filters
		/// </summary>
		public void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
        {
            D.Filters = new MODULE_MODEL();
            btnRefresh_Click(null, null);
        }

        /// <summary>
        /// Preview
        /// </summary>
        public void btnPreview_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.PREVIEW) as Window;
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		public void btnNew_Click(object sender, MouseButtonEventArgs e)
        {
            var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), null, Commands.Type.NEW) as Window;
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        public void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.DUPLICATE) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        public void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.EDIT) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        public void btnDelete_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = W.dgList.SelectedItems.Cast<MODULE_MODEL>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (dynamic instance in selectedInstances)
                    SQL.DeleteInstance(D.MODULE_TYPE, instance.ID, instance.Name);
                btnRefresh_Click(null, null);
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        public async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            await Task.Run(() => {
                UpdateFilters();
                D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL);
                D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, D.Page = 0);
            });
        }

        /// <summary>
        /// Close
        /// </summary>
        public void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            W.Close();
        }

        /// <summary>
        /// Select
        /// </summary>
        public void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!D.SelectingMode)
                {
                    if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.SAVE}"))
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
        public void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                W.DataContext = null;
                D.InstancesList.AddRange(SQL.ListInstances(D.MODULE_TYPE, D.FilterSQL, D.SORTING, ++D.Page).DataTableToList<MODULE_MODEL>());
                W.DataContext = D;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        public void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
