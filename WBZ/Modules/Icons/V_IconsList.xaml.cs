using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
	/// <summary>
	/// Interaction logic for IconsList.xaml
	/// </summary>
	public partial class IconsList : List
	{
		readonly D_IconsList D = new D_IconsList();

		public IconsList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
