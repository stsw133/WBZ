using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    class D_EmployeesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.EMPLOYEES;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowy pracownik";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie pracownika: {InstanceInfo.Fullname}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja pracownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd pracownika: {InstanceInfo.Fullname}";
			}
		}
	}
}
