using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    class D_UsersList : D_ModuleList<MODULE_MODEL>
	{
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
