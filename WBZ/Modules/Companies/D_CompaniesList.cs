using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Company;

namespace WBZ.Modules.Companies
{
    class D_CompaniesList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.COMPANIES;
        public StringCollection SORTING = Properties.Settings.Default.sorting_CompaniesList;
    }
}
