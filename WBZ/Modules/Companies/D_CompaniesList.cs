using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Company;

namespace WBZ.Modules.Companies
{
    class D_CompaniesList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.COMPANIES;
        public StringCollection SORTING = Properties.Settings.Default.sorting_CompaniesList;
    }
}
