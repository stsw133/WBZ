using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Documents
{
    /// <summary>
    /// Interaction logic for DocumentsListGrid.xaml
    /// </summary>
    public partial class DocumentsListGrid : DataGrid
    {
        public DocumentsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Documents_PanelColor);
		}
    }
}
