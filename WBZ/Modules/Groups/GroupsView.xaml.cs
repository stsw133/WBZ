﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules.Groups;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for GroupsView.xaml
    /// </summary>
    public partial class GroupsView : TreeView
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

        /// <summary>
        /// Loaded
        /// </summary>
        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
        /// GetParentItem
        /// </summary>
        TreeViewItem GetParentItem(TreeViewItem item)
        {
            for (var i = VisualTreeHelper.GetParent(item); i != null; i = VisualTreeHelper.GetParent(i))
                if (i is TreeViewItem)
                    return (TreeViewItem)i;
            return null;
        }

        /// <summary>
		/// New
		/// </summary>
		private void btnGroupsMegaNew_Click(object sender, RoutedEventArgs e)
        {
            var instance = new M_Group()
            {
                Module = Module,
                Owner = 0
            };
            
            new GroupsNew(instance, Commands.Type.NEW) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
        /// New
        /// </summary>
        private void btnGroupsNew_Click(object sender, RoutedEventArgs e)
        {
            var path = (((SelectedItem as TreeViewItem).Header as StackPanel).Children[1] as TextBlock).Text + "\\";
            for (var i = GetParentItem((SelectedItem as TreeViewItem)); i != null; i = GetParentItem(i))
                path = ((i.Header as StackPanel).Children[1] as TextBlock).Text + "\\" + path;
            if (path.Split('\\').Length > 5)
                return;

            var instance = new M_Group()
            {
                Module = Module,
                Owner = (int)(SelectedItem as TreeViewItem).Tag,
                Path = path
            };

            new GroupsNew(instance, Commands.Type.NEW) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
		/// Preview
		/// </summary>
		private void btnGroupsPreview_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            int id = (int)(SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(Global.Module.GROUPS, id), Commands.Type.PREVIEW) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
		/// Edit
		/// </summary>
		private void btnGroupsEdit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            int id = (int)(SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(Global.Module.GROUPS, id), Commands.Type.EDIT) { Owner = Window.GetWindow(this) }.ShowDialog();
            btnGroupsRefresh_Click(null, null);
        }

        /// <summary>
        /// Delete
        /// </summary>
        private void btnGroupsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            if (MessageBox.Show("Czy na pewno usunąć zaznaczoną grupę?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var group = SelectedItem as TreeViewItem;
                SQL.DeleteInstance(Global.Module.GROUPS, (int)group.Tag, ((group.Header as StackPanel).Children[1] as TextBlock).Text);
                btnGroupsRefresh_Click(null, null);
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
                Window win = Window.GetWindow(this);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    InstancesList = SQL.ListInstances<M_Group>(Global.Module.GROUPS, $"g.module='{Module}' and g.instance is null");
                }

                /// Clear groups
                Items.Clear();

                TreeViewItem GetTreeViewHeader(M_Group group)
                {
                    var stack = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    var image = new Image()
                    {
                        Source = Functions.LoadImage(group.Icon),
                        Margin = new Thickness(0, 0, 5, 0),
                        Width = Properties.Settings.Default.config_iSize * 1.5,
                        Height = Properties.Settings.Default.config_iSize * 1.5
                    };
                    image.Visibility = image.Source != null ? Visibility.Visible : Visibility.Collapsed;
                    var tb = new TextBlock()
                    {
                        Text = group.Name
                    };
                    if (group.Archival)
                        tb.Foreground = (Brush)new BrushConverter().ConvertFrom("#777");
                    stack.Children.Add(image);
                    stack.Children.Add(tb);
                    var tvi = new TreeViewItem()
                    {
                        Tag = group.ID,
                        Header = stack
                    };
                    return tvi;
                }

                /// Add groups to TreeView
                foreach (var group1 in InstancesList.FindAll(x => x.Owner == 0))
                {
                    var tvi1 = GetTreeViewHeader(group1);
                    Items.Add(tvi1);

                    foreach (var group2 in InstancesList.FindAll(x => x.Owner == group1.ID))
                    {
                        var tvi2 = GetTreeViewHeader(group2);
                        tvi1.Items.Add(tvi2);

                        foreach (var group3 in InstancesList.FindAll(x => x.Owner == group2.ID))
                        {
                            var tvi3 = GetTreeViewHeader(group3);
                            tvi2.Items.Add(tvi3);

                            foreach (var group4 in InstancesList.FindAll(x => x.Owner == group3.ID))
                            {
                                var tvi4 = GetTreeViewHeader(group4);
                                tvi3.Items.Add(tvi4);

                                foreach (var group5 in InstancesList.FindAll(x => x.Owner == group4.ID))
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
        /// SelectedItemChanged
        /// </summary>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    if (e.NewValue != null)
                        d.Filters.Group = (int)(e.NewValue as TreeViewItem).Tag;
                    else
                        d.Filters.Group = 0;
                    (win as dynamic).cmdRefresh_Executed(null, null);
                }
            }
            catch { }
        }
    }
}
