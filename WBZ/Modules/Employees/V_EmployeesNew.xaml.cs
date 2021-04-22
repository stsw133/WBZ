using WBZ.Controls;
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

        /// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
        {
            if (string.IsNullOrEmpty(D.InstanceInfo.Forename))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano imienia!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceInfo.Lastname))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwiska!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

	public class New : ModuleNew<MODULE_MODEL> { }
}
