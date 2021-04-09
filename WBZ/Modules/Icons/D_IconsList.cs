using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
    class D_IconsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = M_Module.Module.ICONS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_IconsList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór ikony";
				else								return "Lista ikon";
			}
		}
	}
}
