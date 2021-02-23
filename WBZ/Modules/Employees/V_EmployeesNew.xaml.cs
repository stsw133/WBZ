using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesNew.xaml
    /// </summary>
    public partial class EmployeesNew : New
    {
        D_EmployeesNew D = new D_EmployeesNew();

        public EmployeesNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
