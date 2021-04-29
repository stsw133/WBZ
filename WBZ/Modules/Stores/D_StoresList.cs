using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_TransportList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.STORES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_StoresList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista magazynów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór magazynu";
				else									return string.Empty;
			}
		}
	}
}
