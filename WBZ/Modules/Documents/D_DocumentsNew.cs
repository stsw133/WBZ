using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.DOCUMENTS;
		
		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.NEW)
					return "Nowy dokument";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)
					return $"Edycja dokumentu: {InstanceInfo.Name}";
				else
					return $"Podgląd dokumentu: {InstanceInfo.Name}";
			}
		}
	}
}
