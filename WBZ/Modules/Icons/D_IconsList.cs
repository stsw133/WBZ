using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
    class D_IconsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string Module = Config.Modules.ICONS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_IconsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista ikon";
				else if (Mode == Commands.Type.SELECT)	return "Wybór ikony";
				else									return string.Empty;
			}
		}
	}
}
