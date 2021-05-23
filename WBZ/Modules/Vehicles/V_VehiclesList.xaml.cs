using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
	/// <summary>
	/// Interaction logic for VehiclesList.xaml
	/// </summary>
	public partial class VehiclesList : List
	{
        readonly D_VehiclesList D = new D_VehiclesList();

		public VehiclesList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
