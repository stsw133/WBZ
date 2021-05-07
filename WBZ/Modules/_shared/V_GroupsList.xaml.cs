using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Logika interakcji dla klasy GroupsList.xaml
    /// </summary>
    public partial class GroupsList : Window
    {
        D_GroupsList D = new D_GroupsList();

        public GroupsList(string module, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;

            D.Mode = mode;
            D.Module = module;
        }
        
        /// <summary>
        /// Select
        /// </summary>
        public MODULE_MODEL Selected;
        private void groupsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (D.Mode == StswExpress.Globals.Commands.Type.SELECT)
                {
                    var item = groupsView.SelectedItem as TreeViewItem;
                    Selected = SQL.GetInstance<MODULE_MODEL>(Config.Modules.GROUPS, (int)item.Tag);

                    if (Selected != null)
                        DialogResult = true;
                }
            }
        }
    }
}
