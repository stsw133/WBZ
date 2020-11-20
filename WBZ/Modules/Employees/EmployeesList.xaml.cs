using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesList.xaml
    /// </summary>
    public partial class EmployeesList : List
    {
        D_EmployeesList D = new D_EmployeesList();

        public EmployeesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();
            btnRefresh_Click(null, null);

			D.Mode = mode;
		}

		/// Selected
		public MODULE_MODEL Selected;
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
