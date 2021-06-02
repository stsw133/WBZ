using StswExpress;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesNew.xaml
	/// </summary>
	public partial class ArticlesNew : New
	{
		D_ArticlesNew D = new D_ArticlesNew();

		public ArticlesNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceData = instance;
			D.Mode = mode;

			D.InstanceData.Measures = SQL.GetInstancePositions(D.Module, D.InstanceData.ID);
			if (D.Mode == Commands.Type.DUPLICATE)
				foreach (DataRow row in D.InstanceData.Measures.Rows)
					row.SetAdded();
		}

		/// <summary>
		/// Measures - CellEditEnding
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgMeasures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			int index = e.Row.GetIndex();
			dgMeasures_Convert(index);
		}

		private async void dgMeasures_Convert(int index)
		{
			await Task.Delay(10);
			await Task.Run(() => {
				if (D.InstanceData.ID != 0 && index < D.InstanceData.Measures.Rows.Count)
				{
					double conv = Convert.IsDBNull(D.InstanceData.Measures.Rows[index]["converter"]) ? 1 : (double)D.InstanceData.Measures.Rows[index]["converter"];
					D.InstanceData.Measures.Rows[index]["amount"] = Convert.ToDouble(D.InstanceData.AmountRaw) / conv;
					D.InstanceData.Measures.Rows[index]["reserved"] = Convert.ToDouble(D.InstanceData.ReservedRaw) / conv;
				}
			});
		}

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name?.EndsWith("_Stores") == true)
			{
				if (D.InstanceData.ID != 0 && D.InstanceSources_Stores == null)
                    D.InstanceSources_Stores = SQL.ListInstances<M_Store>(Config.Modules.STORES, $"sa.article={D.InstanceData.ID}");
			}
			else if (tab?.Name?.EndsWith("_Documents") == true)
			{
				if (D.InstanceData.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.Modules.DOCUMENTS, $"dp.article={D.InstanceData.ID}");
			}
			else if (tab?.Name?.EndsWith("_Distributions") == true)
			{
				if (D.InstanceData.ID != 0 && D.InstanceSources_Distributions == null)
                    D.InstanceSources_Distributions = SQL.ListInstances<M_Distribution>(Config.Modules.DISTRIBUTIONS, $"dp.article={D.InstanceData.ID}");
			}
		}

		/// <summary>
		/// Open: Store
		/// </summary>
		private void dgList_Stores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            dgSourceList_MouseDoubleClick<M_Store>(sender, e, Config.Modules.STORES);
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            dgSourceList_MouseDoubleClick<M_Document>(sender, e, Config.Modules.DOCUMENTS);
		}

		/// <summary>
		/// Open: Distribution
		/// </summary>
		private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            dgSourceList_MouseDoubleClick<M_Distribution>(sender, e, Config.Modules.DISTRIBUTIONS);
		}

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
