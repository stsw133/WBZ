using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
		D_LogsList D = new D_LogsList();

		public LogsList()
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (Config.Logs_Enabled == "1")
				chckEnabled.IsChecked = true;
			else
				chckEnabled.IsChecked = false;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public override void UpdateFilters()
		{
			int index = 0;
			tcList.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => index = tcList.SelectedIndex));

			D.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{D.Filters.cUser.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.content,'')) like '%{D.Filters.Content.ToLower()}%' and "
						+ $"l.datetime >= '{D.Filters.fDateTime:yyyy-MM-dd}' and l.datetime < '{D.Filters.DateTime.AddDays(1):yyyy-MM-dd}' and "
						+ $"l.type={index + 1} and ";

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Loaded
		/// </summary>
		internal override void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (D.SelectingMode)
			{
				dgList_Logs.SelectionMode = DataGridSelectionMode.Single;
				dgList_Errors.SelectionMode = DataGridSelectionMode.Single;
			}

			tcList.SelectedIndex = 1;
			UpdateFilters();
			D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL);
			D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, D.Page = 0);
			tcList.SelectedIndex = 0;

			cmdRefresh_Executed(null, null);
		}

		/// <summary>
		/// Preview
		/// </summary>
		internal override void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (tcList.SelectedIndex == 0) ///logs
			{
				var selectedInstances = dgList_Logs.SelectedItems.Cast<MODULE_MODEL>();
				foreach (MODULE_MODEL instance in selectedInstances)
					Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.PREVIEW);
			}
			else if (tcList.SelectedIndex == 1) ///errors
			{
				var selectedInstances = dgList_Errors.SelectedItems.Cast<MODULE_MODEL>();
				foreach (MODULE_MODEL instance in selectedInstances)
					Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.PREVIEW);
			}
		}

		/// <summary>
		/// Edit
		/// </summary>
		internal override void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (tcList.SelectedIndex == 0) ///logs
			{
				var selectedInstances = dgList_Logs.SelectedItems.Cast<MODULE_MODEL>();
				foreach (MODULE_MODEL instance in selectedInstances)
					Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.EDIT);
			}
			else if (tcList.SelectedIndex == 1) ///errors
			{
				var selectedInstances = dgList_Errors.SelectedItems.Cast<MODULE_MODEL>();
				foreach (MODULE_MODEL instance in selectedInstances)
					Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.EDIT);
			}
		}

		/// <summary>
		/// Select
		/// </summary>
		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
			if (D.InstancesList == null)
				return;

			if ((sender as TabControl).SelectedIndex == 0) ///logs
			{
				D.InstancesList_Errors = D.InstancesList;
				D.InstancesList = D.InstancesList_Logs;
				D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL.Replace("l.type=2", "l.type=1"));
			}
			else if ((sender as TabControl).SelectedIndex == 1) ///errors
			{
				D.InstancesList_Logs = D.InstancesList;
				D.InstancesList = D.InstancesList_Errors;
				D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL.Replace("l.type=1", "l.type=2"));
			}
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
