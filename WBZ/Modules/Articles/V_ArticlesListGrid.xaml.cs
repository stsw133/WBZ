using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for ArticlesListGrid.xaml
    /// </summary>
    public partial class ArticlesListGrid : DataGrid
    {
        public ArticlesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Articles);
		}
    }
}
