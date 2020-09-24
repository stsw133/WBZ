﻿using System;
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

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresList.xaml
	/// </summary>
	public partial class StoresList : Window
	{
		M_StoresList M = new M_StoresList();

		public StoresList(bool selectingMode = false)
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
			M.FilterSQL = $"LOWER(COALESCE(s.codename,'')) like '%{M.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.name,'')) like '%{M.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.postcode,'')) like '%{M.Filters.Postcode.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.city,'')) like '%{M.Filters.City.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.address,'')) like '%{M.Filters.Address.ToLower()}%' and "
						+ (!M.Filters.Archival ? $"s.archival=false and " : "");
			
			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new C_Store();
			btnRefresh_Click(null, null);
		}
		#endregion

		#region buttons
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Store>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new StoresAdd(M.InstancesList[index], false);
				window.Show();
			}
		}
		private void btnAdd_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new StoresAdd(new C_Store(), true);
			window.Show();
		}
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Store>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new StoresAdd(M.InstancesList[index], true);
				window.Show();
			}
		}
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Store>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteStore(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
		}
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
				M.InstancesList = SQL.ListStores(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "name", false);
			});
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		public C_Store Selected;
		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!M.SelectingMode)
				{
					if (Global.User.Perms.Contains($"{M.INSTANCE_TYPE}_{Global.UserPermType.SAVE}"))
						btnEdit_Click(null, null);
					else
						btnPreview_Click(null, null);
				}
				else
				{
					var indexes = dgList.SelectedItems.Cast<C_Store>().Select(x => M.InstancesList.IndexOf(x));
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
				M.InstancesList.AddRange(SQL.ListStores(M.FilterSQL, M.Limit, ++M.Page * M.Limit, "name", false));
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
	internal class M_StoresList : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.Module.STORES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Dane o liście użytkowników
		public DataTable UsersList { get; } = SQL.GetUsersFullnames();
		/// Lista instancji
		private List<C_Store> instancesList;
		public List<C_Store> InstancesList
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
		private C_Store filters = new C_Store();
		public C_Store Filters
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
