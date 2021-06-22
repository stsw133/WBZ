using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for DistributionsListGrid.xaml
    /// </summary>
    public partial class DistributionsListGrid : DataGrid
    {
        public DistributionsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Distributions);
		}
    }
}
