using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Contractors
{
    /// <summary>
    /// Interaction logic for ContractorsListGrid.xaml
    /// </summary>
    public partial class ContractorsListGrid : DataGrid
    {
        public ContractorsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Contractors_PanelColor);
		}
    }
}
