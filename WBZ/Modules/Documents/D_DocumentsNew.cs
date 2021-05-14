using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowy dokument";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie dokumentu: {InstanceData.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja dokumentu: {InstanceData.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd dokumentu: {InstanceData.Name}";
				else										return string.Empty;
			}
		}
	}
}
