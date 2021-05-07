using StswExpress.Globals;
using StswExpress.Translate;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
    class D_VehiclesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return TM.Tr("VehicleNew");
				else if (Mode == Commands.Type.DUPLICATE)	return TM.Tr("VehicleDuplicate") + $": {InstanceData.Name}";
				else if (Mode == Commands.Type.EDIT)		return TM.Tr("VehicleEdit") + $": {InstanceData.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return TM.Tr("VehiclePreview") + $": {InstanceData.Name}";
				else										return string.Empty;
			}
		}
	}
}
