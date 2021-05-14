using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsList : D_ModuleList<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista dokumentów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór dokumentu";
				else									return string.Empty;
			}
		}
	}
}
