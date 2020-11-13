using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_Company;

namespace WBZ.Modules.Companies
{
    /// <summary>
    /// Logika interakcji dla klasy CompaniesList.xaml
    /// </summary>
    public partial class CompaniesList : Window
    {
        D_CompaniesList D = new D_CompaniesList();

        public CompaniesList(bool selectingMode = false)
        {
            InitializeComponent();
            DataContext = D;
            btnRefresh_Click(null, null);

            D.SelectingMode = selectingMode;
            if (D.SelectingMode)
                dgList.SelectionMode = DataGridSelectionMode.Single;
        }

        /// <summary>
		/// Update filters
		/// </summary>
        private void UpdateFilters()
        {
            D.FilterSQL = $"LOWER(COALESCE(c.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.branch,'')) like '%{D.Filters.Branch.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.nip,'')) like '%{D.Filters.NIP.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.regon,'')) like '%{D.Filters.REGON.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"c.archival=false and " : "");

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
            {
                var window = new CompaniesNew(instance, Commands.Type.PREVIEW);
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		private void btnNew_Click(object sender, MouseButtonEventArgs e)
        {
            var window = new CompaniesNew(new MODULE_CLASS(), Commands.Type.NEW);
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        private void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
            foreach (MODULE_CLASS instance in selectedInstances)
            {
                var window = new CompaniesNew(instance, Commands.Type.DUPLICATE);
                window.Show();
            }
        }

        /// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_CLASS>();
            foreach (MODULE_CLASS instance in selectedInstances)
            {
                var window = new CompaniesNew(instance, Commands.Type.EDIT);
                window.Show();
            }
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
                    SQL.DeleteInstance(D.MODULE_NAME, instance.ID, instance.Name);
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
                D.InstancesList = SQL.ListInstances<MODULE_CLASS>(D.MODULE_NAME, D.FilterSQL, D.SORTING, D.Page = 0);
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
        public MODULE_CLASS Selected;
        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                    Selected = dgList.SelectedItems.Cast<MODULE_CLASS>().FirstOrDefault();
                    DialogResult = true;
                }
            }
        }

        /// <summary>
		/// Load more
		/// </summary>
        private void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                DataContext = null;
                D.InstancesList.AddRange(SQL.ListInstances<MODULE_CLASS>(D.MODULE_NAME, D.FilterSQL, D.SORTING, ++D.Page));
                DataContext = D;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}
