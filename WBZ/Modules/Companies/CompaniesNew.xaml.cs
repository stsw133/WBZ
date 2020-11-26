using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Globals;
using MODULE_MODEL = WBZ.Models.M_Company;
using WBZ.Interfaces;

namespace WBZ.Modules.Companies
{
    /// <summary>
    /// Logika interakcji dla klasy CompaniesNew.xaml
    /// </summary>
    public partial class CompaniesNew : New
    {
        D_CompaniesNew D = new D_CompaniesNew();

        public CompaniesNew(MODULE_MODEL instance, Commands.Type mode)
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
