using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
    class D_AttributesClassesList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.ATTRIBUTES_CLASSES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_AttributesClassesList;
	}
}
