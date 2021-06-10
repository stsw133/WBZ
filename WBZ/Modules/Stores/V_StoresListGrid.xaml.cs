using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Stores
{
    /// <summary>
    /// Interaction logic for StoresListGrid.xaml
    /// </summary>
    public partial class StoresListGrid : DataGrid
    {
        public StoresListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Stores_PanelColor);
        }
    }
}
