using System.Windows;
using WBZ.Globals;
using WBZ.Modules._base;
using WBZ.Modules.Users;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesNew.xaml
    /// </summary>
    public partial class EmployeesNew : New
    {
        D_EmployeesNew D = new D_EmployeesNew();

        public EmployeesNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;
		}

		/// <summary>
		/// Select: User
		/// </summary>
		private void btnSelectUser_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList(Commands.Type.SELECTING);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.User = window.Selected.ID;
					D.InstanceInfo.UserName = window.Selected.Fullname;
					D.InstanceInfo = D.InstanceInfo;
				}
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
