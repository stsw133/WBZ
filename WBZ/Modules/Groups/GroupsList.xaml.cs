using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_Group;

namespace WBZ.Modules.Groups
{
	/// <summary>
	/// Interaction logic for GroupsList.xaml
	/// </summary>
	public partial class GroupsList : Window
	{
		D_GroupsList D = new D_GroupsList();

		public GroupsList(bool selectingMode = false)
		{
			InitializeComponent();
			DataContext = D;
			btnRefresh_Click(null, null);

			D.SelectingMode = selectingMode;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(g.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(g.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"g.archival=false and " : "");

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
		/// New
		/// </summary>
		private void btnNew_Click(object sender, MouseButtonEventArgs e)
		{
			if (string.IsNullOrEmpty(D.Filters.Module))
				return;

			var window = new MsgWin(MsgWin.Type.InputBox, "Nowa grupa", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", "");
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				int owner = 0;
				foreach (var groupName in window.Value.Split('/'))
				{
					var groups = SQL.ListInstances(D.MODULE_NAME, $"g.name='{groupName}'").DataTableToList<MODULE_CLASS>();
					if (groups.Count > 0)
						owner = groups[0].ID;
					else
					{
						var group = new MODULE_CLASS()
						{
							ID = SQL.NewInstanceID(D.MODULE_NAME),
							Module = D.Filters.Module,
							Name = groupName,
							Owner = owner
						};
						SQL.SetInstance(D.MODULE_NAME, group, Global.ActionType.NEW);
						owner = group.ID;
					}
				}
			}
		}

		/// <summary>
		/// Duplicate
		/// </summary>
		private void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstance = tvGroups.SelectedItem as MODULE_CLASS;
			if (selectedInstance == null)
				return;
			
			var window = new MsgWin(MsgWin.Type.InputBox, "Nowa grupa", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", selectedInstance.Fullpath);
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				int owner = 0;
				foreach (var groupName in window.Value.Split('/'))
				{
					var groups = SQL.ListInstances(D.MODULE_NAME, $"g.name='{groupName}'").DataTableToList<MODULE_CLASS>();
					if (groups.Count > 0)
						owner = groups[0].ID;
					else
					{
						var group = new MODULE_CLASS()
						{
							ID = SQL.NewInstanceID(D.MODULE_NAME),
							Module = D.Filters.Module,
							Name = groupName,
							Owner = owner
						};
						SQL.SetInstance(D.MODULE_NAME, group, Global.ActionType.DUPLICATE);
						owner = group.ID;
					}
				}
			}
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstance = tvGroups.SelectedItem as MODULE_CLASS;
			if (selectedInstance == null)
				return;

			var window = new MsgWin(MsgWin.Type.InputBox, $"Edycja grupy: {selectedInstance.Name}", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", selectedInstance.Fullpath);
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				int owner = 0;
				foreach (var groupName in window.Value.Split('/'))
				{
					var groups = SQL.ListInstances(D.MODULE_NAME, $"g.name='{groupName}'").DataTableToList<MODULE_CLASS>();
					if (groups.Count > 0)
						owner = groups[0].ID;
					else
					{
						var group = new MODULE_CLASS()
						{
							ID = selectedInstance.ID,
							Module = D.Filters.Module,
							Name = groupName,
							Owner = owner
						};
						SQL.SetInstance(D.MODULE_NAME, group, Global.ActionType.EDIT);
						owner = group.ID;
					}
				}
			}
		}

		/// <summary>
		/// Delete
		/// </summary>
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstance = tvGroups.SelectedItem as MODULE_CLASS;
			if (selectedInstance == null)
				return;

			if (MessageBox.Show("Czy na pewno usunąć zaznaczony rekord?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				SQL.DeleteInstance(D.MODULE_NAME, selectedInstance.ID, selectedInstance.Name);
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
				D.InstancesList = SQL.ListInstances(D.MODULE_NAME, D.FilterSQL, D.SORTING).DataTableToList<MODULE_CLASS>();
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
}
