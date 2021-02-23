using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
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

			if (M_Config.Logs_Enabled == "1")
				chckEnabled.IsChecked = true;
			else
				chckEnabled.IsChecked = false;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{D.Filters.UserFullname.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.content,'')) like '%{D.Filters.Content.ToLower()}%' and "
						+ $"l.datetime >= '{D.Filters.fDateTime:yyyy-MM-dd}' and l.datetime <= '{D.Filters.DateTime:yyyy-MM-dd}' and "
						+ $"l.type={D.Filters.Group} and ";

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Preview
		/// </summary>
		private void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, StswExpress.Globals.Commands.Type.EDIT);
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
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
