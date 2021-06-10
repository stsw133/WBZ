using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Logs
{
    /// <summary>
    /// Interaction logic for LogsListGrid.xaml
    /// </summary>
    public partial class LogsListGrid : DataGrid
    {
        public LogsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Logs_PanelColor);
		}
    }
}
