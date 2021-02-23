using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.DISTRIBUTIONS;
		
		/// Window title
		public string Title
		{
			get
			{
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowa dystrybucja";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie dystrybucji: {InstanceInfo.Name}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja dystrybucji: {InstanceInfo.Name}";
				else
					return $"Podgląd dystrybucji: {InstanceInfo.Name}";
			}
		}
	}
}
