using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    class D_UsersList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.USERS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_UsersList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista użytkowników";
				else if (Mode == Commands.Type.SELECT)	return "Wybór użytkownika";
				else									return string.Empty;
			}
		}
	}
}
