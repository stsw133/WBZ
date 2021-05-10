using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
    class D_VehiclesList : D_ModuleList<MODULE_MODEL>
	{
		/// Sorting
		public override StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_VehiclesList;
			set => Properties.Settings.Default.sorting_VehiclesList = value;
		}
	}
}
