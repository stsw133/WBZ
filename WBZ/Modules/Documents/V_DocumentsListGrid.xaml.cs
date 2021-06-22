using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for DocumentsListGrid.xaml
    /// </summary>
    public partial class DocumentsListGrid : DataGrid
    {
        public DocumentsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Documents);
		}
    }
}
