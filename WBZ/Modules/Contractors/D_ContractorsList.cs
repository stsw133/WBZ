using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsList : D_ModuleList<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = M_Module.Module.CONTRACTORS;
        public StringCollection SORTING = Properties.Settings.Default.sorting_ContractorsList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór kontrahenta";
				else								return "Lista kontrahentów";
			}
		}
	}
}
