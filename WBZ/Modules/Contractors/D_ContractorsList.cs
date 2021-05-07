using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string Module = Config.Modules.CONTRACTORS;
        public StringCollection SORTING = Properties.Settings.Default.sorting_ContractorsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista kontrahentów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór kontrahenta";
				else									return string.Empty;
			}
		}
	}
}
