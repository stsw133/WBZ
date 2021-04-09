using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = M_Module.Module.FAMILIES;
        public StringCollection SORTING = Properties.Settings.Default.sorting_FamiliesList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór rodziny";
				else								return "Lista rodzin";
			}
		}
	}
}
