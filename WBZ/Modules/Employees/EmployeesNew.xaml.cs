using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Helpers;
using WBZ.Modules.Users;
using MODULE_CLASS = WBZ.Models.C_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesNew.xaml
    /// </summary>
    public partial class EmployeesNew : Window
    {
        D_EmployeesNew D = new D_EmployeesNew();

        public EmployeesNew(MODULE_CLASS instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;

            D.InstanceInfo = instance;
			D.Mode = mode;

			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
		}

		/// <summary>
		/// Validation
		/// </summary>
		private bool CheckDataValidation()
		{
			bool result = true;

			return result;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (D.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Select: User
		/// </summary>
		private void btnSelectUser_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList(true);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.User = window.Selected.ID;
					D.InstanceInfo.UserName = window.Selected.Fullname;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
