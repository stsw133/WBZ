using StswExpress;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._submodules.Groups;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._submodules
{
    /// <summary>
    /// Logika interakcji dla klasy GroupsList.xaml
    /// </summary>
    public partial class GroupsList : Window
    {
        readonly D_GroupsList D = new D_GroupsList();

        public GroupsList(MV module, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;

            D.Module = module;
            D.Mode = mode;
        }

        /// <summary>
        /// Select
        /// </summary>
        public MODULE_MODEL Selected;
        private void GroupsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (D.Mode == Commands.Type.SELECT)
                {
                    if (GroupsView.SelectedItem is TreeViewItem itm)
                        Selected = SQL.GetInstance<MODULE_MODEL>(Config.GetModule(nameof(Groups)), (int)itm.Tag);

                    if (Selected != null)
                        DialogResult = true;
                }
            }
        }
    }
}
