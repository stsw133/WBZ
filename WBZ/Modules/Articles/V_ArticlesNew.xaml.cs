using StswExpress;
using System;
using System.Data;
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
        readonly D_ArticlesNew D = new D_ArticlesNew();

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
        private void DtgMeasures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var index = e.Row.GetIndex();
            DtgMeasures_Convert(index);
        }
        private void DtgMeasures_Convert(int index)
        {
            if (D.InstanceData.ID != 0 && index < D.InstanceData.Measures.Rows.Count)
            {
                var conv = Convert.IsDBNull(D.InstanceData.Measures.Rows[index]["converter"]) ? 1d : (double)D.InstanceData.Measures.Rows[index]["converter"];
                D.InstanceData.Measures.Rows[index]["quantity"] = Convert.ToDouble(D.InstanceData.QuantityRaw) / conv;
                D.InstanceData.Measures.Rows[index]["reserved"] = Convert.ToDouble(D.InstanceData.ReservedRaw) / conv;
            }
        }

        /// <summary>
        /// Tab changed for source
        /// </summary>
        private void TabConSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name?.EndsWith($"_{nameof(Distributions)}") == true)
            {
                if (D.InstanceData.ID != 0 && D.InstanceSources_Distributions == null)
                    D.InstanceSources_Distributions = SQL.ListInstances<M_Distribution>(Config.GetModule(nameof(Distributions)), $"dp.article={D.InstanceData.ID}");
            }
            else if (tab?.Name?.EndsWith($"_{nameof(Documents)}") == true)
            {
                if (D.InstanceData.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.GetModule(nameof(Documents)), $"dp.article={D.InstanceData.ID}");
            }
            else if (tab?.Name?.EndsWith($"_{nameof(Stores)}") == true)
            {
                if (D.InstanceData.ID != 0 && D.InstanceSources_Stores == null)
                    D.InstanceSources_Stores = SQL.ListInstances<M_Store>(Config.GetModule(nameof(Stores)), $"sa.article={D.InstanceData.ID}");
            }
        }
        private void DtgDistributions_MouseDoubleClick(object sender, MouseButtonEventArgs e) => DtgSourceList_MouseDoubleClick<M_Distribution>(sender, e, Config.GetModule(nameof(Distributions)));
        private void DtgDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e) => DtgSourceList_MouseDoubleClick<M_Document>(sender, e, Config.GetModule(nameof(Documents)));
        private void DtgStores_MouseDoubleClick(object sender, MouseButtonEventArgs e) => DtgSourceList_MouseDoubleClick<M_Store>(sender, e, Config.GetModule(nameof(Stores)));

        /// <summary>
        /// Validation
        /// </summary>
        internal override bool CheckDataValidation()
        {
            if (string.IsNullOrEmpty(D.InstanceData.Name))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
