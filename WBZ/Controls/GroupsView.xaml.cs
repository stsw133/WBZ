using StswExpress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._submodules;
using WBZ.Modules._submodules.Groups;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for GroupsView.xaml
    /// </summary>
    public partial class GroupsView : TreeView
    {
        internal List<M_Group> InstancesList = new List<M_Group>();
        private MV WindowModule, GroupModule = Config.GetModule(nameof(Modules._submodules.Groups));

        public GroupsView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// EditingMode
		/// </summary>
		public bool EditingMode => Config.User.Perms.Contains($"{WindowModule.Name}_{Config.PermType.GROUPS}");

        /// <summary>
        /// Loaded
        /// </summary>
        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            if (d != null)
                WindowModule = d.Module;

            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
		/// Preview
		/// </summary>
		private void btnGroupsPreview_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || (int)(SelectedItem as Control).Tag == 0)
                return;

            int id = (int)(SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(GroupModule, id), Commands.Type.PREVIEW) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
		/// New
		/// </summary>
		private void btnGroupsNew_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem GetParentItem(TreeViewItem item)
            {
                for (var i = VisualTreeHelper.GetParent(item); i != null; i = VisualTreeHelper.GetParent(i))
                    if (i is TreeViewItem)
                        return (TreeViewItem)i;
                return null;
            }

            var instance = new M_Group();
            if (SelectedItem == null || (int)(SelectedItem as Control).Tag == 0)
            {
                instance.Module = WindowModule;
                instance.OwnerID = 0;
            }
            else
            {
                var path = (((SelectedItem as TreeViewItem).Header as StackPanel).Children[1] as TextBlock).Text + "\\";
                for (var i = GetParentItem(SelectedItem as TreeViewItem); i != null; i = GetParentItem(i))
                    path = ((i.Header as StackPanel).Children[1] as TextBlock).Text + "\\" + path;

                if (path.Split('\\').Length > 5)
                    return;

                instance.Module = WindowModule;
                instance.OwnerID = (int)(SelectedItem as TreeViewItem).Tag;
                instance.Path = path;
            }
            new GroupsNew(instance, Commands.Type.NEW) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
		/// Edit
		/// </summary>
		private void btnGroupsEdit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || (int)(SelectedItem as Control).Tag == 0)
                return;

            int id = (int)(SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(GroupModule, id), Commands.Type.EDIT) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
        /// Delete
        /// </summary>
        private void btnGroupsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || (int)(SelectedItem as Control).Tag == 0)
                return;

            if (MessageBox.Show("Czy na pewno usunąć zaznaczoną grupę?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var group = SelectedItem as TreeViewItem;
                SQL.DeleteInstance(GroupModule, (int)group.Tag, ((group.Header as StackPanel).Children[1] as TextBlock).Text);
                btnGroupsRefresh_Click(null, null);
            }
        }

        /// <summary>
        /// Expand all
        /// </summary>
        private void btnGroupsExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem tvi1 in Items)
            {
                tvi1.IsExpanded = Properties.Settings.Default.config_ExpandGroups;
                foreach (TreeViewItem tvi2 in tvi1.Items)
                {
                    tvi2.IsExpanded = Properties.Settings.Default.config_ExpandGroups;
                    foreach (TreeViewItem tvi3 in tvi2.Items)
                    {
                        tvi3.IsExpanded = Properties.Settings.Default.config_ExpandGroups;
                        foreach (TreeViewItem tvi4 in tvi3.Items)
                        {
                            tvi4.IsExpanded = Properties.Settings.Default.config_ExpandGroups;
                            foreach (TreeViewItem tvi5 in tvi4.Items)
                            {
                                tvi5.IsExpanded = Properties.Settings.Default.config_ExpandGroups;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
		/// Refresh
		/// </summary>
		private void btnGroupsRefresh_Click(object sender, RoutedEventArgs e)
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
                // TODO - filtr do parametryzacji
                InstancesList = SQL.ListInstances<M_Group>(GroupModule, $"{GroupModule.Alias}.module_alias='{WindowModule.Alias}' and {GroupModule.Alias}.instance_id is null");
                Items.Clear();

                TreeViewItem GetTreeViewHeader(M_Group group)
                {
                    var stack = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    var image = new Image()
                    {
                        Source = Fn.LoadImage(group.IconContent),
                        Margin = new Thickness(0, 0, 5, 0),
                        Width = Settings.Default.iSize * 1.5,
                        Height = Settings.Default.iSize * 1.5
                    };
                    image.Visibility = image.Source != null ? Visibility.Visible : Visibility.Collapsed;
                    var tb = new TextBlock()
                    {
                        Text = group.Name
                    };
                    if (group.IsArchival)
                        tb.Foreground = (Brush)new BrushConverter().ConvertFrom("#777");
                    stack.Children.Add(image);
                    stack.Children.Add(tb);
                    var tvi = new TreeViewItem()
                    {
                        Tag = group.ID,
                        Header = stack,
                        IsExpanded = Properties.Settings.Default.config_ExpandGroups
                    };
                    if (!string.IsNullOrEmpty(group.Comment))
                        tvi.ToolTip = group.Comment;
                    return tvi;
                }

                /// Add groups to TreeView
                Items.Add(GetTreeViewHeader(new M_Group() { Name = ". . ." }));
                foreach (var group1 in InstancesList.Where(x => x.OwnerID == 0))
                {
                    var tvi1 = GetTreeViewHeader(group1);
                    Items.Add(tvi1);

                    foreach (var group2 in InstancesList.Where(x => x.OwnerID == group1.ID))
                    {
                        var tvi2 = GetTreeViewHeader(group2);
                        tvi1.Items.Add(tvi2);

                        foreach (var group3 in InstancesList.Where(x => x.OwnerID == group2.ID))
                        {
                            var tvi3 = GetTreeViewHeader(group3);
                            tvi2.Items.Add(tvi3);

                            foreach (var group4 in InstancesList.Where(x => x.OwnerID == group3.ID))
                            {
                                var tvi4 = GetTreeViewHeader(group4);
                                tvi3.Items.Add(tvi4);

                                foreach (var group5 in InstancesList.Where(x => x.OwnerID == group4.ID))
                                {
                                    var tvi5 = GetTreeViewHeader(group5);
                                    tvi4.Items.Add(tvi5);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// PreviewMouseDoubleClick
        /// </summary>
        private void TreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this).GetType() == typeof(GroupsList))
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Config.User.Perms.Contains($"{WindowModule.Name}_{Config.PermType.SAVE}"))
                    btnGroupsEdit_Click(null, null);
                else
                    btnGroupsPreview_Click(null, null);
            }
        }

        /// <summary>
        /// PreviewMouseRightButtonDown
        /// </summary>
        private TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
                treeViewItem.Focus();
            else if (SelectedItem != null)
                (SelectedItem as TreeViewItem).IsSelected = false;
            e.Handled = true;
        }

        /// <summary>
        /// SelectedItemChanged
        /// </summary>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Window win = Window.GetWindow(this);
                var d = win?.DataContext as D_GroupsList;
                if (d != null)
                {
                    d.Filter.ShowGroup = e.NewValue != null ? (int)(e.NewValue as TreeViewItem).Tag : 0;
                    Commands.Refresh.Execute(null, win);
                }
            }
            catch { }
        }
    }
}
