using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Modules.Companies
{
    /// <summary>
    /// Logika interakcji dla klasy CompaniesList.xaml
    /// </summary>
    public partial class CompaniesList : Window
    {
        M_CompaniesList M = new M_CompaniesList();

        public CompaniesList(bool selectingMode = false)
        {
            InitializeComponent();
            DataContext = M;
            btnRefresh_Click(null, null);

            M.SelectingMode = selectingMode;
            if (M.SelectingMode)
                dgList.SelectionMode = DataGridSelectionMode.Single;
        }

        /// <summary>
        /// Aktualizacja filtrów wyszukiwania
        /// </summary>
        #region filters
        private void UpdateFilters()
        {
            M.FilterSQL = $"LOWER(COALESCE(c.codename,'')) like '%{M.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{M.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.branch,'')) like '%{M.Filters.Branch.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.nip,'')) like '%{M.Filters.NIP.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.regon,'')) like '%{M.Filters.REGON.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.postcode,'')) like '%{M.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.city,'')) like '%{M.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.address,'')) like '%{M.Filters.Address.ToLower()}%' and "
                        + (!M.Filters.Archival ? $"c.archival=false and " : "");

            M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
        }
        private void dpFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnRefresh_Click(null, null);
        }
        private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
        {
            M.Filters = new C_Company();
            btnRefresh_Click(null, null);
        }
        #endregion

        #region buttons
        private void btnPreview_Click(object sender, MouseButtonEventArgs e)
        {
            var indexes = dgList.SelectedItems.Cast<C_Company>().Select(x => M.InstancesList.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new CompaniesAdd(M.InstancesList[index], false);
                window.Show();
            }
        }
        private void btnAdd_Click(object sender, MouseButtonEventArgs e)
        {
            var window = new CompaniesAdd(new C_Company(), true);
            window.Show();
        }
        private void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            var indexes = dgList.SelectedItems.Cast<C_Company>().Select(x => M.InstancesList.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new CompaniesAdd(M.InstancesList[index], true);
                window.Show();
            }
        }
        private void btnDelete_Click(object sender, MouseButtonEventArgs e)
        {
            var indexes = dgList.SelectedItems.Cast<C_Company>().Select(x => M.InstancesList.IndexOf(x));
            if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (int index in indexes)
                    SQL.DeleteCompany(M.InstancesList[index].ID);
                btnRefresh_Click(null, null);
            }
        }
        private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            await Task.Run(() => {
                UpdateFilters();
                M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
                M.InstancesList = SQL.ListCompanies(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "name", false);
            });
        }
        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        #endregion

        public C_Company Selected;
        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!M.SelectingMode)
                {
                    if (Global.User.Perms.Contains($"{M.INSTANCE_TYPE}_{Global.UserPermTypes.SAVE}"))
                        btnEdit_Click(null, null);
                    else
                        btnPreview_Click(null, null);
                }
                else
                {
                    var indexes = dgList.SelectedItems.Cast<C_Company>().Select(x => M.InstancesList.IndexOf(x));
                    foreach (int index in indexes)
                        Selected = M.InstancesList[index];

                    DialogResult = true;
                }
            }
        }

        private void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && M.InstancesList.Count < M.TotalItems)
            {
                DataContext = null;
                M.InstancesList.AddRange(SQL.ListCompanies(M.FilterSQL, M.Limit, ++M.Page * M.Limit, "name", false));
                DataContext = M;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }

    /// <summary>
	/// Model
	/// </summary>
	internal class M_CompaniesList : INotifyPropertyChanged
    {
        public readonly string INSTANCE_TYPE = Global.ModuleTypes.COMPANIES;

        /// Dane o zalogowanym użytkowniku
        public C_User User { get; } = Global.User;
        /// Dane o liście użytkowników
        public DataTable UsersList { get; } = SQL.GetUsersFullnames();
        /// Lista instancji
        private List<C_Company> instancesList;
        public List<C_Company> InstancesList
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
        /// Tryb wyboru dla okna
        public bool SelectingMode { get; set; }
        /// Filtr SQL
        public string FilterSQL { get; set; }
        /// Instancja filtra
        private C_Company filters = new C_Company();
        public C_Company Filters
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
        /// Numer strony
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
        /// Limit rekordów na stronę
        public int Limit { get; } = 50;
        /// Łączna liczba elementów w module
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
