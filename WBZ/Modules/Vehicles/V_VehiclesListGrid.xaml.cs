using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for VehiclesListGrid.xaml
    /// </summary>
    public partial class VehiclesListGrid : DataGrid
    {
        public VehiclesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Vehicles_PanelColor);
		}
    }
}
