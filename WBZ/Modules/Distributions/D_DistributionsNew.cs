using StswExpress.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowa dystrybucja";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie dystrybucji: {InstanceData.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja dystrybucji: {InstanceData.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd dystrybucji: {InstanceData.Name}";
				else										return string.Empty;
			}
		}
	}
}
