using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresList.xaml
	/// </summary>
	public partial class StoresList : List
	{
		D_StoresList D = new D_StoresList();

		public StoresList(Commands.Type mode)
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
