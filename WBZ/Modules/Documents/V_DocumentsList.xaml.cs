using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsList.xaml
	/// </summary>
	public partial class DocumentsList : List
	{
		readonly D_DocumentsList D = new D_DocumentsList();

		public DocumentsList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
