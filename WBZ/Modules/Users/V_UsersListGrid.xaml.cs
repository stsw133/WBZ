using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Users
{
    /// <summary>
    /// Interaction logic for UsersListGrid.xaml
    /// </summary>
    public partial class UsersListGrid : DataGrid
    {
        public UsersListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Users_PanelColor);
		}
    }
}
