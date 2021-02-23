using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

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
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowy dokument";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja dokumentu: {InstanceInfo.Name}";
				else
					return $"Podgląd dokumentu: {InstanceInfo.Name}";
			}
		}
	}
}
