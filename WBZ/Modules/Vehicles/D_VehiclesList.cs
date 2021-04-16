using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
    class D_VehiclesList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.VEHICLES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_VehiclesList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór pojazdu";
				else								return "Lista pojazdów";
			}
		}
	}
}
