using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
    class D_AttachmentsGallery : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.ATTACHMENTS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_AttachmentsGallery;
	}
}
