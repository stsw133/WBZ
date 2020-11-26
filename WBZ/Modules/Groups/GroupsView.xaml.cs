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

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnGroupRefresh_Click(null, null);
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
		private void btnGroupNew_Click(object sender, MouseButtonEventArgs e)
        {
            var instance = new M_Group();
            if (tvGroups.SelectedItem == null)
            {
                instance.Module = Module;
                instance.Owner = 0;
            }
            else
            {
                var fullpath = (tvGroups.SelectedItem as TreeViewItem).Header.ToString() + "\\";
                for (var i = GetParentItem((tvGroups.SelectedItem as TreeViewItem)); i != null; i = GetParentItem(i))
                    fullpath = i.Header + "\\" + fullpath;

                if (fullpath.Split('\\').Length > 5)
                    return;

                instance.Module = Module;
                instance.Owner = (int)(tvGroups.SelectedItem as TreeViewItem).Tag;
                instance.Fullpath = fullpath;
            }
            new GroupsNew(instance, Commands.Type.NEW) { Owner = Window.GetWindow(this) }.Show();
        }

        /// <summary>
		/// Duplicate
		/// </summary>
		private void btnGroupDuplicate_Click(object sender, MouseButtonEventArgs e)
        {
            if (tvGroups.SelectedItem == null)
                return;

            int id = (int)(tvGroups.SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(Global.Module.GROUPS, id), Commands.Type.DUPLICATE) { Owner = Window.GetWindow(this) }.Show();
        }

        /// <summary>
		/// Edit
		/// </summary>
		private void btnGroupEdit_Click(object sender, MouseButtonEventArgs e)
        {
            if (tvGroups.SelectedItem == null)
                return;

            int id = (int)(tvGroups.SelectedItem as TreeViewItem).Tag;
            new GroupsNew(SQL.GetInstance<M_Group>(Global.Module.GROUPS, id), Commands.Type.EDIT) { Owner = Window.GetWindow(this) }.Show();
        }

        /// <summary>
        /// Delete
        /// </summary>
        private void btnGroupDelete_Click(object sender, MouseButtonEventArgs e)
        {
            if (tvGroups.SelectedItem == null)
                return;

            if (MessageBox.Show("Czy na pewno usunąć zaznaczoną grupę?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var group = tvGroups.SelectedItem as TreeViewItem;
                SQL.DeleteInstance(Global.Module.GROUPS, (int)group.Tag, (string)group.Header);
                btnGroupRefresh_Click(null, null);
            }
        }

        /// <summary>
		/// Refresh
		/// </summary>
		private void btnGroupRefresh_Click(object sender, MouseButtonEventArgs e)
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
                tvGroups.Items.Clear();
                foreach (var group1 in InstancesList.FindAll(x => x.Owner == 0))
                {
                    var tvi1 = new TreeViewItem()
                    {
                        Tag = group1.ID,
                        Header = group1.Name
                    };
                    tvGroups.Items.Add(tvi1);

                    foreach (var group2 in InstancesList.FindAll(x => x.Owner == group1.ID))
                    {
                        var tvi2 = new TreeViewItem()
                        {
                            Tag = group2.ID,
                            Header = group2.Name
                        };
                        tvi1.Items.Add(tvi2);

                        foreach (var group3 in InstancesList.FindAll(x => x.Owner == group2.ID))
                        {
                            var tvi3 = new TreeViewItem()
                            {
                                Tag = group3.ID,
                                Header = group3.Name
                            };
                            tvi2.Items.Add(tvi3);

                            foreach (var group4 in InstancesList.FindAll(x => x.Owner == group3.ID))
                            {
                                var tvi4 = new TreeViewItem()
                                {
                                    Tag = group4.ID,
                                    Header = group4.Name
                                };
                                tvi3.Items.Add(tvi4);

                                foreach (var group5 in InstancesList.FindAll(x => x.Owner == group4.ID))
                                {
                                    var tvi5 = new TreeViewItem()
                                    {
                                        Tag = group5.ID,
                                        Header = group5.Name
                                    };
                                    tvi4.Items.Add(tvi5);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
