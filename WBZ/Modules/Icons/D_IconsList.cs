using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
    class D_IconsList : D_ModuleList<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista ikon";
				else if (Mode == Commands.Type.SELECT)	return "Wybór ikony";
				else									return string.Empty;
			}
		}

		/// Sorting
		public override StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_IconsList;
			set => throw new System.NotImplementedException();
		}
	}
}
