﻿using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsList : D_ModuleList<MODULE_MODEL>
    {
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

		/// Sorting
		public override StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_ContractorsList;
			set => throw new System.NotImplementedException();
		}
	}
}
