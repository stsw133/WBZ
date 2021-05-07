using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string Module = Config.Modules.DOCUMENTS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_DocumentsList;

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
