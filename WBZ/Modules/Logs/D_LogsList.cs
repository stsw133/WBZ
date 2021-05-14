using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Log;

namespace WBZ.Modules.Logs
{
    class D_LogsList : D_ModuleList<MODULE_MODEL>
	{
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
