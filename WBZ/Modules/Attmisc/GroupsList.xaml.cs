using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Controls;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Classes.C_Group;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for GroupsList.xaml
	/// </summary>
	public partial class GroupsList : Window
	{
		M_GroupsList M = new M_GroupsList();

		public GroupsList(bool selectingMode = false)
		{
			InitializeComponent();
			DataContext = M;
			btnRefresh_Click(null, null);

			M.SelectingMode = selectingMode;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			M.FilterSQL = $"LOWER(COALESCE(g.module,'')) like '%{M.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(g.name,'')) like '%{M.Filters.Name.ToLower()}%' and "
						+ (!M.Filters.Archival ? $"g.archival=false and " : "");

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
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
			M.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// New
		/// </summary>
		private void btnNew_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new MsgWin(MsgWin.Type.InputBox, "Nowa grupa", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", "");
			if (window.ShowDialog() == true)
			{
				
			}
		}

		/// <summary>
		/// Duplicate
		/// </summary>
		private void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new AttributesClassesAdd(M.InstancesList[index], true);
				window.Show();

				var window = new MsgWin(MsgWin.Type.InputBox, $"Duplikowanie grupy: {M.InstancesList[index].Fullpath}", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", M.InstancesList[index].Fullpath);
				if (window.ShowDialog() == true)
				{

				}
			}*/
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new AttributesClassesAdd(M.InstancesList[index], true);
				window.Show();

				var window = new MsgWin(MsgWin.Type.InputBox, $"Edycja grupy: {M.InstancesList[index].Fullpath}", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", M.InstancesList[index].Fullpath);
				if (window.ShowDialog() == true)
				{

				}
			}*/
		}

		/// <summary>
		/// Delete
		/// </summary>
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteAttributeClass(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
			*/
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				//UpdateFilters();
				//M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
				//M.InstancesList = SQL.ListAttributesClasses(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "name", false);
			});
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
    }

	/// <summary>
	/// Model
	/// </summary>
	internal class M_GroupsList : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.GROUPS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_GroupsList;

		/// Logged user
		public C_User User { get; } = Global.User;
		/// Instances list
		private List<MODULE_CLASS> instancesList;
		public List<MODULE_CLASS> InstancesList
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
		/// Selecting mode
		public bool SelectingMode { get; set; }
		/// SQL filter
		public string FilterSQL { get; set; }
		/// Filter instance
		private MODULE_CLASS filters = new MODULE_CLASS();
		public MODULE_CLASS Filters
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
		/// Total instances number
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
