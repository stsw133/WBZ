using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
    class D_IconsList : D_ModuleList<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista klas atrybutów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór klasy atrybutu";
				else									return string.Empty;
			}
		}

		/// Sorting
		public override StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_AttributesClassesList;
			set => throw new System.NotImplementedException();
		}
	}
}
