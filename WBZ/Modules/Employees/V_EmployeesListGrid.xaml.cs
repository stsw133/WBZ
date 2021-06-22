using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for EmployeesListGrid.xaml
    /// </summary>
    public partial class EmployeesListGrid : DataGrid
    {
        public EmployeesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Employees);
		}
    }
}
