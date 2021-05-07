using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
    class D_AttachmentsList : D_ModuleList<MODULE_MODEL>
    {
		/// Module
		public readonly string Module = Config.Modules.ATTACHMENTS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_AttachmentsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista załączników";
				else if (Mode == Commands.Type.SELECT)	return "Wybór załącznika";
				else									return string.Empty;
			}
		}
	}
}
