using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for GroupsView.xaml
    /// </summary>
    public partial class GroupsView : UserControl
    {
        public List<M_Group> InstancesList = new List<M_Group>();
        private string Module;

        public GroupsView()
        {
            InitializeComponent();
        }

        public bool EditingMode
        {
            get { return (bool)GetValue(pEditingMode); }
            set { SetValue(pEditingMode, value); }
        }
        public static readonly DependencyProperty pEditingMode
            = DependencyProperty.Register(
                  nameof(EditingMode),
                  typeof(bool),
                  typeof(GroupsView),
                  new PropertyMetadata(false)
              );

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemCollection recurseItemCollection(ItemCollection items, int id)
            {
                ItemCollection result = null;
                foreach (TreeViewItem subItem in items)
                {
                    if (Convert.ToInt32(subItem.Tag) != id)
                        result = recurseItemCollection(subItem.Items, id);
                    else
                        return subItem.Items;
                }
                return result;
            }

            try
            {
                Window win = Window.GetWindow(this);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    InstancesList = SQL.ListInstances<M_Group>(Global.Module.GROUPS, $"g.module='{Module}' and g.instance is null");
                }

                /// Add groups to TreeView
                var items = tvGroups.Items;
                foreach (var group in InstancesList)
                {
                    foreach (var _ in (group.Fullpath + '\\').Split('\\'))
                    {
                        var owner = group;
                        TreeViewItem tvi;

                        do
                        {
                            var currents = recurseItemCollection(items, owner.ID);
                            if (currents == null && owner.Owner != 0)
                                owner = SQL.GetInstance<M_Group>(Global.Module.GROUPS, owner.Owner);
                            else if (currents != null)
                            {
                                tvi = new TreeViewItem()
                                {
                                    Tag = owner.ID,
                                    Header = owner.Name
                                };
                                currents.Add(tvi);
                            }
                        } while (owner.Owner != 0);

                        if (recurseItemCollection(items, owner.ID) == null)
                        {
                            tvi = new TreeViewItem()
                            {
                                Tag = owner.ID,
                                Header = owner.Name
                            };
                            items.Add(tvi);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
		/// Add
		/// </summary>
		private void btnGroupAdd_Click(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
		/// Edit
		/// </summary>
		private void btnGroupEdit_Click(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Remove
        /// </summary>
        private void btnGroupRemove_Click(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
