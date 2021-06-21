using StswExpress;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Log;

namespace WBZ.Modules.Logs
{
	/// <summary>
	/// Interaction logic for LogsList.xaml
	/// </summary>
	public partial class LogsList : List
	{
		readonly D_LogsList D = new D_LogsList();

		public LogsList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;

			if (Config.Logs_Enabled == "1")
				chckEnabled.IsChecked = true;
			else
				chckEnabled.IsChecked = false;
		}

		/// <summary>
		/// GetDataGrid
		/// </summary>
		private DataGrid GetDataGrid(int selectedTab)
        {
			if		(selectedTab == 0)	return dgList_Logs;
			else if (selectedTab == 1)	return dgList_Errors;
			else						return null;
        }

		/// <summary>
		/// Update filters
		/// </summary>
		internal override void UpdateFilters()
		{
			base.UpdateFilters();
			D.Filter.AutoFilterString += $" and l.type={D.SelectedTab + 1}";
			if (D.Filter.AutoFilterString.StartsWith(" and "))
				D.Filter.AutoFilterString = D.Filter.AutoFilterString[5..];
		}

		/// <summary>
		/// Loaded
		/// </summary>
		internal override void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (D.Mode == Commands.Type.SELECT)
			{
				dgList_Logs.SelectionMode = DataGridSelectionMode.Single;
				dgList_Errors.SelectionMode = DataGridSelectionMode.Single;
			}
			cmdRefresh_Executed(null, null);
		}

		/// <summary>
		/// Preview
		/// </summary>
		internal override void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = GetDataGrid(D.SelectedTab).SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		internal override void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = GetDataGrid(D.SelectedTab).SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.EDIT);
		}

		/// <summary>
		/// Select
		/// </summary>
		internal override void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				cmdEdit_Executed(null, null);
		}

		/// <summary>
		/// Logs enable/disable
		/// </summary>
		private void chckEnabled_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
				SQL.SetPropertyValue("LOGS_ENABLED", "1");
			else
				SQL.SetPropertyValue("LOGS_ENABLED", "0");
		}

		/// <summary>
		/// SelectionChanged
		/// </summary>
        private void tcList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			cmdRefresh_Executed(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
