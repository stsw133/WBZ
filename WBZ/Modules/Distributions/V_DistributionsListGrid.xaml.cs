using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Distributions
{
    /// <summary>
    /// Interaction logic for DistributionsListGrid.xaml
    /// </summary>
    public partial class DistributionsListGrid : DataGrid
    {
        public DistributionsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Distributions_PanelColor);
		}
    }
}
