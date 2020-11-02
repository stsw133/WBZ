using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Helpers;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Stores;
using MODULE_CLASS = WBZ.Models.C_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesNew.xaml
	/// </summary>
	public partial class ArticlesNew : Window
	{
		D_ArticlesNew D = new D_ArticlesNew();

		public ArticlesNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = D;

			D.InstanceInfo = instance;
			D.Mode = mode;

			D.InstanceInfo.Measures = SQL.GetArticleMeasures(D.InstanceInfo.ID);
			if (D.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
			{
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
				foreach (DataRow row in D.InstanceInfo.Measures.Rows)
					row.SetAdded();
			}
		}

		/// <summary>
		/// Validation
		/// </summary>
		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (D.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		private void dgMeasures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			int index = e.Row.GetIndex();

			dgMeasures_Convert(index);
		}

		private async void dgMeasures_Convert(int index)
		{
			await Task.Delay(10);
			await Task.Run(() => {
				if (D.InstanceInfo.ID != 0 && index < D.InstanceInfo.Measures.Rows.Count)
				{
					double conv = Convert.IsDBNull(D.InstanceInfo.Measures.Rows[index]["converter"]) ? 1 : (double)D.InstanceInfo.Measures.Rows[index]["converter"];
					D.InstanceInfo.Measures.Rows[index]["amount"] = Convert.ToDouble(D.InstanceInfo.AmountRaw) / conv;
					D.InstanceInfo.Measures.Rows[index]["reserved"] = Convert.ToDouble(D.InstanceInfo.ReservedRaw) / conv;
				}
			});
		}

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Stores")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Stores == null)
					D.InstanceSources_Stores = SQL.ListInstances(Global.Module.STORES, $"sa.article={D.InstanceInfo.ID}").DataTableToList<C_Store>();
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Documents == null)
					D.InstanceSources_Documents = SQL.ListInstances(Global.Module.DOCUMENTS, $"dp.article={D.InstanceInfo.ID}").DataTableToList<C_Document>();
			}
			else if (tab?.Name == "tabSources_Distributions")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Distributions == null)
					D.InstanceSources_Distributions = SQL.ListInstances(Global.Module.DISTRIBUTIONS, $"dp.article={D.InstanceInfo.ID}").DataTableToList<C_Distribution>();
			}
		}

		/// <summary>
		/// Open: Store
		/// </summary>
		private void dgList_Stores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Store>();
				foreach (C_Store instance in selectedInstances)
				{
					var window = new StoresNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Document>();
				foreach (C_Document instance in selectedInstances)
				{
					var window = new DocumentsNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Distribution
		/// </summary>
		private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Distribution>();
				foreach (C_Distribution instance in selectedInstances)
				{
					var window = new DistributionsNew(instance, perm);
					window.Show();
				}
			}
		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			if (D.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
