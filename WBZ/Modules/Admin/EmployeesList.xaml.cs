using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using INSTANCE_CLASS = WBZ.Classes.C_Employee;

namespace WBZ.Modules.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesList.xaml
    /// </summary>
    public partial class EmployeesList : Window
    {
        M_EmployeesList M = new M_EmployeesList();

        public EmployeesList(bool selectingMode = false)
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
			M.FilterSQL = $"LOWER(COALESCE(e.forename,'')) like '%{M.Filters.Forename.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.lastname,'')) like '%{M.Filters.Lastname.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.department,'')) like '%{M.Filters.Department.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.position,'')) like '%{M.Filters.Position.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.email,'')) like '%{M.Filters.Email.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.phone,'')) like '%{M.Filters.Phone.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.postcode,'')) like '%{M.Filters.Postcode.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.city,'')) like '%{M.Filters.City.ToLower()}%' and "
						+ $"LOWER(COALESCE(e.address,'')) like '%{M.Filters.Address.ToLower()}%' and "
						+ (M.Filters.Archival ? $"e.archival=false and " : "");

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new INSTANCE_CLASS();
			btnRefresh_Click(null, null);
		}
		#endregion

		#region buttons
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<INSTANCE_CLASS>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new EmployeesAdd(M.InstancesList[index], false);
				window.Show();
			}
		}
		private void btnAdd_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new EmployeesAdd(new INSTANCE_CLASS(), true);
			window.Show();
		}
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<INSTANCE_CLASS>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new EmployeesAdd(M.InstancesList[index], true);
				window.Show();
			}
		}
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<INSTANCE_CLASS>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteEmployee(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
		}
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
				M.InstancesList = SQL.ListEmployees(M.FilterSQL, M.Page = 0);
			});
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		public INSTANCE_CLASS Selected;
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
					var indexes = dgList.SelectedItems.Cast<INSTANCE_CLASS>().Select(x => M.InstancesList.IndexOf(x));
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
				M.InstancesList.AddRange(SQL.ListEmployees(M.FilterSQL, ++M.Page));
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
	internal class M_EmployeesList : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.ModuleTypes.EMPLOYEES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Lista instancji
		private List<INSTANCE_CLASS> instancesList;
		public List<INSTANCE_CLASS> InstancesList
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
		private INSTANCE_CLASS filters = new INSTANCE_CLASS();
		public INSTANCE_CLASS Filters
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
		/// Łączna liczba instancji
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
