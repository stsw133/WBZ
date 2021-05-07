using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Log;

namespace WBZ.Modules.Logs
{
    class D_LogsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.LOGS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_LogsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista logów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór logu";
				else									return string.Empty;
			}
		}
	}
}
