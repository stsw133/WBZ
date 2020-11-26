using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;

namespace WBZ.Interfaces
{
    interface IModuleList
	{
        void Init();
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

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            W = GetWindow(this);
            D = W.DataContext;
            FullName = W.GetType().FullName;
            HalfName = FullName.Substring(0, FullName.Length - 4);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D.SelectingMode)
                W.dgList.SelectionMode = DataGridSelectionMode.Single;
        }

        /// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
        {
            dynamic Filters = D.Filters;

            switch (D.MODULE_TYPE)
            {
                /// ARTICLES
                case Global.Module.ARTICLES:
                    D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.name,'')) like '%{Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(a.ean,'')) like '%{Filters.EAN.ToLower()}%' and "
                        + (!Filters.Archival ? $"a.archival=false and " : "")
                        + (W.SelectedStore?.ID > 0 ? $"sa.store={W.SelectedStore.ID} and " : "");
                    break;
                /// ATTRIBUTES_CLASSES
                case Global.Module.ATTRIBUTES_CLASSES:
                    D.FilterSQL = $"LOWER(COALESCE(ac.module,'')) like '%{Filters.Module.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.name,'')) like '%{Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.type,'')) like '%{Filters.Type.ToLower()}%' and "
                        + $"LOWER(COALESCE(ac.values,'')) like '%{Filters.Values.ToLower()}%' and "
                        + (!Filters.Archival ? $"ac.archival=false and " : "");
                    break;
                /// COMPANIES
                case Global.Module.COMPANIES:
                    D.FilterSQL = $"LOWER(COALESCE(c.codename,'')) like '%{Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.branch,'')) like '%{Filters.Branch.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.nip,'')) like '%{Filters.NIP.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.regon,'')) like '%{Filters.REGON.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.postcode,'')) like '%{Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.city,'')) like '%{Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.address,'')) like '%{Filters.Address.ToLower()}%' and "
                        + (!Filters.Archival ? $"c.archival=false and " : "");
                    break;
                /// DISTRIBUTIONS
                case Global.Module.DISTRIBUTIONS:
                    D.FilterSQL = $"LOWER(COALESCE(d.name,'')) like '%{Filters.Name}%' and "
                        + $"d.datereal between '{Filters.fDateReal:yyyy-MM-dd}' and '{Filters.DateReal:yyyy-MM-dd} 23:59:59' and "
                        //+ (M.Filters.FamiliesCount > 0 ? $"COALESCE(count(family),0) = {M.Filters.FamiliesCount} and " : "")
                        + (!Filters.Archival ? $"d.archival=false and " : "");
                    break;
                /// DOCUMENTS
                case Global.Module.DOCUMENTS:
                    D.FilterSQL = $"LOWER(COALESCE(d.type,'')) like '%{Filters.Type.ToLower()}%' and "
                        + $"LOWER(COALESCE(d.name,'')) like '%{Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.name,'')) like '%{Filters.StoreName.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{Filters.CompanyName.ToLower()}%' and "
                        + $"d.dateissue between '{Filters.fDateIssue:yyyy-MM-dd}' and '{Filters.DateIssue:yyyy-MM-dd} 23:59:59' and "
                        + (!Filters.Archival ? $"d.archival=false and " : "");
                    break;
                /// EMPLOYEES
                case Global.Module.EMPLOYEES:
                    D.FilterSQL = $"LOWER(COALESCE(e.forename,'')) like '%{Filters.Forename.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.lastname,'')) like '%{Filters.Lastname.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.department,'')) like '%{Filters.Department.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.position,'')) like '%{Filters.Position.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.email,'')) like '%{Filters.Email.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.phone,'')) like '%{Filters.Phone.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.postcode,'')) like '%{Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.city,'')) like '%{Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.address,'')) like '%{Filters.Address.ToLower()}%' and "
                        + (Filters.Archival ? $"e.archival=false and " : "");
                    break;
                /// FAMILIES
                case Global.Module.FAMILIES:
                    D.FilterSQL = $"LOWER(COALESCE(f.declarant,'')) like '%{Filters.Declarant.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.lastname,'')) like '%{Filters.Lastname.ToLower()}%' and "
                        + (Filters.Members > 0 ? $"COALESCE(f.members,0) = {Filters.Members} and " : "")
                        + $"LOWER(COALESCE(f.postcode,'')) like '%{Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.city,'')) like '%{Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.address,'')) like '%{Filters.Address.ToLower()}%' and "
                        + (!Filters.Archival ? $"f.archival=false and " : "");
                    break;
                /// STORES
                case Global.Module.STORES:
                    D.FilterSQL = $"LOWER(COALESCE(s.codename,'')) like '%{Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.name,'')) like '%{Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.postcode,'')) like '%{Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.city,'')) like '%{Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(s.address,'')) like '%{Filters.Address.ToLower()}%' and "
                        + (!Filters.Archival ? $"s.archival=false and " : "");
                    break;
                /// USERS
                case Global.Module.USERS:
                    D.FilterSQL = $"LOWER(COALESCE(u.forename,'')) like '%{Filters.Forename.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.lastname,'')) like '%{Filters.Lastname.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.email,'')) like '%{Filters.Email.ToLower()}%' and "
                        + $"LOWER(COALESCE(u.phone,'')) like '%{Filters.Phone.ToLower()}%' and "
                        + (Filters.Archival ? $"u.archival=false and " : "");
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
                D.InstancesList.AddRange(SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, ++D.Page));
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
