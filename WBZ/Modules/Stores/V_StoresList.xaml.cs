using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresList.xaml
	/// </summary>
	public partial class StoresList : List
	{
        readonly D_StoresList D = new D_StoresList();

		public StoresList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
