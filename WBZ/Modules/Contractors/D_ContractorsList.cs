using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.CONTRACTORS;
        public StringCollection SORTING = Properties.Settings.Default.sorting_ContractorsList;
    }
}
