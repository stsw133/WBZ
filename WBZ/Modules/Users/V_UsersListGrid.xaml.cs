using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for UsersListGrid.xaml
    /// </summary>
    public partial class UsersListGrid : DataGrid
    {
        public UsersListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Users);
		}
    }
}
