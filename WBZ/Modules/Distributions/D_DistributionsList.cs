using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string Module = Config.Modules.DISTRIBUTIONS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_DistributionsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista dystrybucji";
				else if (Mode == Commands.Type.SELECT)	return "Wybór dystrybucji";
				else									return string.Empty;
			}
		}
	}
}
