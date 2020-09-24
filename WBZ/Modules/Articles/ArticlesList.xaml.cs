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

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ItemsList.xaml
	/// </summary>
	public partial class ArticlesList : Window
	{
		M_ArticlesList M = new M_ArticlesList();

		public ArticlesList(bool selectingMode = false)
		{
			InitializeComponent();
			DataContext = M;
			if (M.StoresList.Rows.Count > 0)
				cbStore.SelectedIndex = 0;

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
			M.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{M.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{M.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.ean,'')) like '%{M.Filters.EAN.ToLower()}%' and "
						+ (!M.Filters.Archival ? $"a.archival=false and " : "");

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new C_Article();
			btnRefresh_Click(null, null);
		}
		#endregion

		#region buttons
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Article>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new ArticlesAdd(M.InstancesList[index], false);
				window.Show();
			}
		}
		private void btnAdd_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new ArticlesAdd(new C_Article(), true);
			window.Show();
		}
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Article>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new ArticlesAdd(M.InstancesList[index], true);
				window.Show();
			}
		}
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Article>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteArticle(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
		}
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
				M.InstancesList = SQL.ListArticles(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "name", false, SelectedStore?.ID ?? 0);
			});
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance("stores", cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0) as C_Store;
			btnRefresh_Click(null, null);
		}

		public C_Store SelectedStore;
		public C_Article Selected;
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
					var indexes = dgList.SelectedItems.Cast<C_Article>().Select(x => M.InstancesList.IndexOf(x));
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
				M.InstancesList.AddRange(SQL.ListArticles(M.FilterSQL, M.Limit, ++M.Page * M.Limit, "name", false, (int)cbStore.SelectedValue));
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
	internal class M_ArticlesList : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.Module.ARTICLES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Dane o liście użytkowników
		public DataTable UsersList { get; } = SQL.GetUsersFullnames();
		/// Dane o liście magazynów
		public DataTable StoresList { get; } = SQL.GetStoresNames();
		/// Lista instancji
		private List<C_Article> instancesList;
		public List<C_Article> InstancesList
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
		private C_Article filters = new C_Article();
		public C_Article Filters
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
