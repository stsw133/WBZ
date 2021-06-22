using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for IconsListGrid.xaml
    /// </summary>
    public partial class IconsListGrid : DataGrid
    {
        public IconsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Icons);
		}
    }
}
