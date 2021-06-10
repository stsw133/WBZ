using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Interaction logic for FamiliesListGrid.xaml
    /// </summary>
    public partial class FamiliesListGrid : DataGrid
    {
        public FamiliesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Families_PanelColor);
		}
    }
}
