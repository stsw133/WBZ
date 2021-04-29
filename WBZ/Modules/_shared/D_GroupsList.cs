using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    class D_GroupsList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public string MODULE_TYPE = Config.Modules.GROUPS;
        public StringCollection SORTING = Properties.Settings.Default.sorting_GroupsList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór grupy";
				else								return "Lista grup";
			}
		}
	}
}
