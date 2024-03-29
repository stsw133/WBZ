﻿using StswExpress;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules._base
{
    public abstract class ModuleList<MODULE_MODEL> : Window where MODULE_MODEL : class, new()
    {
        private Window W;
        private D_ModuleList<MODULE_MODEL> D;
        private string Namespace;
        private List<DataGrid> dgLists = new List<DataGrid>();

        /// <summary>
        /// Init
        /// </summary>
        internal void Init()
        {
            W = GetWindow(this);
            D = W.DataContext as D_ModuleList<MODULE_MODEL>;
            Namespace = W.GetType().FullName[0..^4];
        }

        /// <summary>
        /// Loaded
        /// </summary>
        internal virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /// for every datagrid set items source
            dgLists = new List<DataGrid>(Extensions.FindVisualChildren<DataGrid>(W));
            for (int i = 0; i < dgLists.Count; i++)
                D.InstancesLists.Add(new List<MODULE_MODEL>());

            /// for select mode
            if (D.Mode == Commands.Type.SELECT)
                foreach (var dg in dgLists)
                    dg.SelectionMode = DataGridSelectionMode.Single;

            /// refresh
			if (Properties.Settings.Default.config_LoadStartDataOnLists)
                CmdRefresh_Executed(null, null);
            else
                D.TotalItems = SQL.CountInstances(D.Module, D.Filter);
        }

        /// <summary>
		/// Update filters
		/// </summary>
        internal virtual void UpdateFilters()
        {
            /// get column filters
            Fn.GetColumnFilters(dgLists[D.SelectedTab], out var a, out var b);
            D.Filter.AutoFilterString = a;
            D.Filter.AutoFilterParams = new List<MV>();
            foreach (var param in b)
                D.Filter.AutoFilterParams.Add(new MV()
                {
                    Name = param.name,
                    Value = param.val
                });

            /// wbz filters
            if (!D.Filter.ShowArchival) D.Filter.AutoFilterString += $" and {D.Module.Alias}.is_archival=false";
            if (D.Filter.ShowGroup > 0) D.Filter.AutoFilterString += $" and exists (select from wbz.groups g where g.module_alias='{D.Module.Alias}' and g.owner_id={D.Filter.ShowGroup})";
            if (D.Filter.AutoFilterString.StartsWith(" and "))
                D.Filter.AutoFilterString = D.Filter.AutoFilterString[5..];
        }

        /// <summary>
		/// Select instance
		/// </summary>
        internal virtual void CmdSelect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Selected = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>().FirstOrDefault();
            W.DialogResult = true;
        }
        internal void CmdSelect_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = D?.Mode == Commands.Type.SELECT;

        /// <summary>
        /// Preview instances
        /// </summary>
        internal virtual void CmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), SQL.GetInstance<MODULE_MODEL>(D.Module, (instance as M).ID), Commands.Type.PREVIEW) as Window).Show();
        }
        internal void CmdPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Config.User.Perms.Contains($"{D?.Module?.Name}_{Config.PermType.PREVIEW}");

        /// <summary>
		/// New instance
		/// </summary>
        internal virtual void CmdNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (Activator.CreateInstance(Type.GetType(Namespace + "New"), null, Commands.Type.NEW) as Window).Show();
        }
        internal void CmdNew_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Config.User.Perms.Contains($"{D?.Module?.Name}_{Config.PermType.SAVE}");

        /// <summary>
        /// Duplicate instances
        /// </summary>
        internal virtual void CmdDuplicate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), SQL.GetInstance<MODULE_MODEL>(D.Module, (instance as M).ID), Commands.Type.DUPLICATE) as Window).Show();
        }
        internal void CmdDuplicate_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Config.User.Perms.Contains($"{D?.Module?.Name}_{Config.PermType.SAVE}");

        /// <summary>
        /// Edit instances
        /// </summary>
        internal virtual void CmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), SQL.GetInstance<MODULE_MODEL>(D.Module, (instance as M).ID), Commands.Type.EDIT) as Window).Show();
        }
        internal void CmdEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Config.User.Perms.Contains($"{D?.Module?.Name}_{Config.PermType.SAVE}");

        /// <summary>
        /// Delete instances
        /// </summary>
        internal virtual void CmdDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (MODULE_MODEL instance in selectedInstances)
                {
                    SQL.DeleteInstance(D.Module, (instance as M).ID, (instance as M).Name);
                    D.InstancesLists[D.SelectedTab].Remove(instance);
                }
            }
        }
        internal void CmdDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Config.User.Perms.Contains($"{D?.Module?.Name}_{Config.PermType.DELETE}");

        /// <summary>
		/// Clear filters
		/// </summary>
		internal virtual void CmdClear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            D.Filter = new M_Filter(D.Module);
            Fn.ClearColumnFilters(dgLists[D.SelectedTab]);
            CmdRefresh_Executed(null, null);
        }

        /// <summary>
        /// Refresh
        /// </summary>
        internal async virtual void CmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            UpdateFilters();
            await Task.Run(() =>
            {
                D.TotalItems = SQL.CountInstances(D.Module, D.Filter);
                D.InstancesLists[D.SelectedTab] = SQL.ListInstances<MODULE_MODEL>(D.Module, D.Filter, 0);
                D.NotifyPropertyChanged(nameof(D.InstancesLists));
                D.CountItems = D.InstancesLists[D.SelectedTab].Count;
            });
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Close
        /// </summary>
        internal void CmdClose_Executed(object sender, ExecutedRoutedEventArgs e) => W.Close();

        /// <summary>
        /// Select
        /// </summary>
		internal MODULE_MODEL Selected;
        internal virtual void DtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (D.Mode == Commands.Type.LIST)
                {
                    if (Config.User.Perms.Contains($"{D.Module.Name}_{Config.PermType.SAVE}"))
                        CmdEdit_Executed(null, null);
                    else
                        CmdPreview_Executed(null, null);
                }
                else if (D.Mode == Commands.Type.SELECT)
                    CmdSelect_Executed(null, null);
            }
        }

        /// <summary>
        /// Load more
        /// </summary>
        internal async void DtgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesLists[D.SelectedTab].Count < D.TotalItems)
            {
                Cursor = Cursors.Wait;
                await Task.Run(() =>
                {
                    foreach (var i in SQL.ListInstances<MODULE_MODEL>(D.Module, D.Filter, D.InstancesLists[D.SelectedTab].Count))
                        D.InstancesLists[D.SelectedTab].Add(i);
                });
                (e.OriginalSource as ScrollViewer).ScrollToVerticalOffset(e.VerticalOffset);
                Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Sorting
        /// </summary>
        internal void DtgList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var currentSort = CollectionViewSource.GetDefaultView((sender as DataGrid).ItemsSource).SortDescriptions;
            var newSort = $"{D.Module.Alias}.{e.Column.SortMemberPath.ToLower()} {e.Column.SortDirection?.ToString()?.ToLower()?[..^6] ?? "asc"}";

            var sorting = new StringCollection();
            foreach (var sort in currentSort)
                sorting.Add($"{D.Module.Alias}.{sort.PropertyName.ToLower()} {sort.Direction.ToString().ToLower()[..^6]}");
            sorting.Insert(0, newSort);
            D.Filter.Sorting = sorting;
        }

        /// <summary>
        /// Closed
        /// </summary>
        internal void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            if (W.Owner != null)
                W.Owner.Focus();
        }
    }
}
