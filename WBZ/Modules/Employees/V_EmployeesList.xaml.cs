using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesList.xaml
    /// </summary>
    public partial class EmployeesList : List
    {
        readonly D_EmployeesList D = new D_EmployeesList();

        public EmployeesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
