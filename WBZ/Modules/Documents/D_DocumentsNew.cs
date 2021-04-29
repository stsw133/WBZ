using StswExpress.Globals;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.DOCUMENTS;
		
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowy dokument";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja dokumentu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd dokumentu: {InstanceInfo.Name}";
				else										return string.Empty;
			}
		}
	}
}
