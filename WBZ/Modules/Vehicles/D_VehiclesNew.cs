using StswExpress.Globals;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
    class D_VehiclesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.VEHICLES;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowy pojazd";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie pojazdu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja pojazdu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd pojazdu: {InstanceInfo.Name}";
				else										return string.Empty;
			}
		}
	}
}
