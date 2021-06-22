using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for FamiliesListGrid.xaml
    /// </summary>
    public partial class FamiliesListGrid : DataGrid
    {
        public FamiliesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Families);
		}
    }
}
