using StswExpress;
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
        readonly D_VehiclesNew D = new D_VehiclesNew();

		public VehiclesNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			base.Init();

			if (instance != null)
				D.InstanceData = instance;
			D.Mode = mode;
		}

		/// <summary>
		/// Select: Forwarder
		/// </summary>
		private void btnSelectForwarder_Click(object sender, RoutedEventArgs e)
		{
			var window = new ContractorsList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceData.ForwarderID = window.Selected.ID;
					D.InstanceData.ForwarderName = window.Selected.Name;
					D.InstanceData = D.InstanceData;
				}
		}

		/// <summary>
		/// Select: Driver
		/// </summary>
		private void btnSelectDriver_Click(object sender, RoutedEventArgs e)
		{
			var window = new EmployeesList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceData.DriverID = window.Selected.ID;
					D.InstanceData.DriverName = window.Selected.Name;
					D.InstanceData = D.InstanceData;
				}
		}

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceData.Register))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano numeru rejestracyjnego!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
