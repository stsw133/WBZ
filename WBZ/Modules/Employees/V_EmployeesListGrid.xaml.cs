using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesListGrid.xaml
    /// </summary>
    public partial class EmployeesListGrid : DataGrid
    {
        public EmployeesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Employees_PanelColor);
		}
    }
}
