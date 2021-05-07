using StswExpress.Globals;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    class D_EmployeesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string Module = Config.Modules.EMPLOYEES;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowy pracownik";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie pracownika: {InstanceData.Fullname}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja pracownika: {InstanceData.Fullname}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd pracownika: {InstanceData.Fullname}";
				else										return string.Empty;
			}
		}
	}
}
