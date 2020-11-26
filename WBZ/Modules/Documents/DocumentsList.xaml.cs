using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsList.xaml
	/// </summary>
	public partial class DocumentsList : List
	{
		D_DocumentsList D = new D_DocumentsList();

		public DocumentsList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();
			btnRefresh_Click(null, null);

			D.Mode = mode;
		}

		/// Selected
		public MODULE_MODEL Selected;
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
