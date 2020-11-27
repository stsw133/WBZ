using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules.Groups
{
    class D_GroupsList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public string MODULE_TYPE = Global.Module.GROUPS;
        public StringCollection SORTING = Properties.Settings.Default.sorting_GroupsList;
    }
}
