using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
    class D_AttachmentsGallery : D_ModuleList<MODULE_MODEL>
	{
		/// Sorting
        public StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_AttachmentsGallery;
			set => throw new System.NotImplementedException();
		}
    }
}
