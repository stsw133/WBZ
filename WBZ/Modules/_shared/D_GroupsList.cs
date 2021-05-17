﻿using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    class D_GroupsList : D_ModuleList<MODULE_MODEL>
    {
		/// Module
		public new string Module { get; set; }

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista grup";
				else if (Mode == Commands.Type.SELECT)	return "Wybór grupy";
				else									return string.Empty;
			}
		}
	}
}