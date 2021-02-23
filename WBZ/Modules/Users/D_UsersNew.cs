using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    class D_UsersNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.USERS;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowy użytkownik";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie użytkownika: {InstanceInfo.Fullname}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja użytkownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd użytkownika: {InstanceInfo.Fullname}";
			}
		}
	}
}
