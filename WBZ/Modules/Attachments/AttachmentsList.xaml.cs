using System.Linq;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
    /// <summary>
    /// Interaction logic for AttachmentsList.xaml
    /// </summary>
    public partial class AttachmentsList : List
    {
        D_AttachmentsList D = new D_AttachmentsList();

        public AttachmentsList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
			Init();
			cmdRefresh_Executed(null, null);

			D.Mode = mode;
        }

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{D.Filters.UserFullname.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and ";

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Preview
		/// </summary>
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.PREVIEW);
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
			foreach (MODULE_MODEL instance in selectedInstances)
				Functions.OpenInstanceWindow(this, instance, Commands.Type.EDIT);
		}

		/// <summary>
		/// Select
		/// </summary>
		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				btnEdit_Click(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
