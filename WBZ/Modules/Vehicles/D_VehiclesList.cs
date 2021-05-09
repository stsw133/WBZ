using StswExpress.Globals;
using StswExpress.Translate;
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

		/// Title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return TM.Tr("VehiclesList");
				else if (Mode == Commands.Type.SELECT)	return TM.Tr("VehicleSelect");
				else									return string.Empty;
			}
		}
	}
}
