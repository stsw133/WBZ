using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    /// <summary>
    /// Logika interakcji dla klasy ContractorsNew.xaml
    /// </summary>
    public partial class ContractorsNew : New
    {
        D_ContractorsNew D = new D_ContractorsNew();

        public ContractorsNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;
        }

        /// <summary>
		/// Tab changed
		/// </summary>
        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name == "tabSources_Documents")
            {
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Global.Module.DOCUMENTS, $"c.id={D.InstanceInfo.ID}");
            }
        }

        /// <summary>
		/// Open: Document
		/// </summary>
        private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dgList_Module_MouseDoubleClick<M_Document>(sender, e, Global.Module.DOCUMENTS);
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
