using StswExpress;
using System.Linq;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
    /// <summary>
    /// Interaction logic for AttachmentsList.xaml
    /// </summary>
    public partial class AttachmentsList : List
    {
		readonly D_AttachmentsList D = new D_AttachmentsList();

        public AttachmentsList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
			Init();

			D.Mode = mode;
        }

		/// <summary>
		/// Preview
		/// </summary>
		internal override void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		internal override void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.EDIT);
		}

		/// <summary>
		/// Select
		/// </summary>
		internal override void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				cmdEdit_Executed(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
