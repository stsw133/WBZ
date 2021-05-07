using StswExpress.Globals;
using StswExpress.Translate;
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
				if		(Mode == Commands.Type.NEW)			return TM.Tr("VehicleNew");
				else if (Mode == Commands.Type.DUPLICATE)	return TM.Tr("VehicleDuplicate") + $": {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)		return TM.Tr("VehicleEdit") + $": {InstanceInfo.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return TM.Tr("VehiclePreview") + $": {InstanceInfo.Name}";
				else										return string.Empty;
			}
		}
	}
}
