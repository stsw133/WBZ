using StswExpress.Globals;
using System.Windows;
using WBZ.Modules._base;
using WBZ.Modules.Contractors;
using WBZ.Modules.Employees;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
	/// <summary>
	/// Interaction logic for VehiclesNew.xaml
	/// </summary>
	public partial class VehiclesNew : New
	{
		D_VehiclesNew D = new D_VehiclesNew();

		public VehiclesNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;
		}

		/// <summary>
		/// Select: Contractor
		/// </summary>
		private void btnSelectContractor_Click(object sender, RoutedEventArgs e)
		{
			var window = new ContractorsList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.Forwarder = window.Selected.ID;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		/// <summary>
		/// Select: Employee
		/// </summary>
		private void btnSelectEmployee_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.Driver = window.Selected.ID;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceInfo.Register))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano numeru rejestracyjnego!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
