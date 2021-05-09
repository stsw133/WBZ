using StswExpress.Globals;
using System.Collections.Specialized;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    class D_EmployeesList : D_ModuleList<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista pracowników";
				else if (Mode == Commands.Type.SELECT)	return "Wybór pracownika";
				else									return string.Empty;
			}
		}

		/// Sorting
		public override StringCollection Sorting
		{
			get => Properties.Settings.Default.sorting_EmployeesList;
			set => throw new System.NotImplementedException();
		}
	}
}
