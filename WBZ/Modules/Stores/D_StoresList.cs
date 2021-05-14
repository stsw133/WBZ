using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_TransportList : D_ModuleList<MODULE_MODEL>
	{
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
