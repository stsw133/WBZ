using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    class D_GroupsList : D_ModuleList<MODULE_MODEL>
    {
		/// Module
		public new string Module { get; set; }
	}
}
