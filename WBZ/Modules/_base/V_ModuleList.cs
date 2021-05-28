using SE = StswExpress;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using System.Collections.Generic;
using Npgsql;

namespace WBZ.Modules._base
{
    public abstract class ModuleList<MODULE_MODEL> : Window where MODULE_MODEL : class, new()
    {
        private Window W;
        private D_ModuleList<MODULE_MODEL> D;
        private string Namespace;
        private ObservableCollection<DataGrid> dgLists = new ObservableCollection<DataGrid>();

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
            dgLists = new ObservableCollection<DataGrid>(SE.Extensions.FindVisualChildren<DataGrid>(W));
            if (D.Mode == SE.Commands.Type.SELECT)
                foreach (var dg in dgLists)
                    dg.SelectionMode = DataGridSelectionMode.Single;

            if (dgLists.Count > 1)
                D.TotalItems = SQL.CountInstances(D.Module);
            else
                cmdRefresh_Executed(null, null);
        }

        /// <summary>
		/// Update filters
		/// </summary>
        internal virtual void UpdateFilters()
        {
            D.FilterSqlString = string.Empty;
            D.FilterSqlParams = new List<NpgsqlParameter>();

            var dgList = dgLists[D.SelectedTab];
            foreach (var col in dgList.Columns)
                if (col.Header is SE.ColumnFilter c && c?.FilterSQL != null)
                {
                    D.FilterSqlString += c.FilterSQL + " and ";
                    D.FilterSqlParams.Add(new NpgsqlParameter() { ParameterName = c.ParamSQL + "1", Value = c.Value1 ?? DBNull.Value });
                    D.FilterSqlParams.Add(new NpgsqlParameter() { ParameterName = c.ParamSQL + "2", Value = c.Value2 ?? DBNull.Value });
                }
            D.FilterSqlString += !(D.Filters as M).Archival ? $"{Config.GetModuleAlias(D.Module)}.archival=false and " : string.Empty;
            D.FilterSqlString += (D.Filters as M).Group > 0 ? $"exists (select from wbz.groups g where g.instance={Config.GetModuleAlias(D.Module)}.id and g.owner={(D.Filters as M).Group}) and " : string.Empty;
            D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
        }

        /// <summary>
		/// Select instance
		/// </summary>
		internal void cmdSelect_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = D?.Mode == SE.Commands.Type.SELECT;
        internal virtual void cmdSelect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Selected = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>().FirstOrDefault();
            W.DialogResult = true;
        }

        /// <summary>
        /// Preview instances
        /// </summary>
        internal void cmdPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.Module}_{Global.PermType.PREVIEW}");
        internal virtual void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), instance, SE.Commands.Type.PREVIEW) as Window).Show();
        }

        /// <summary>
		/// New instance
		/// </summary>
		internal void cmdNew_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.Module}_{Global.PermType.SAVE}");
        internal virtual void cmdNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (Activator.CreateInstance(Type.GetType(Namespace + "New"), null, SE.Commands.Type.NEW) as Window).Show();
        }

        /// <summary>
        /// Duplicate instances
        /// </summary>
        internal void cmdDuplicate_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.Module}_{Global.PermType.SAVE}");
        internal virtual void cmdDuplicate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), instance, SE.Commands.Type.DUPLICATE) as Window).Show();
        }

        /// <summary>
        /// Edit instances
        /// </summary>
        internal void cmdEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.Module}_{Global.PermType.SAVE}");
        internal virtual void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
                (Activator.CreateInstance(Type.GetType(Namespace + "New"), instance, SE.Commands.Type.EDIT) as Window).Show();
        }

        /// <summary>
        /// Delete instances
        /// </summary>
        internal void cmdDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.Module}_{Global.PermType.DELETE}");
        internal virtual void cmdDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgLists[D.SelectedTab].SelectedItems.Cast<MODULE_MODEL>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (object instance in selectedInstances)
                {
                    SQL.DeleteInstance(D.Module, (instance as M).ID, (instance as M).Name);
                    dgLists[D.SelectedTab].Items.Remove(instance);
                }
            }
        }

        /// <summary>
		/// Clear filters
		/// </summary>
		internal virtual void cmdClear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            D.Filters = new MODULE_MODEL();

            var dgList = dgLists[D.SelectedTab];
            foreach (var col in dgList.Columns)
                if (col.Header is SE.ColumnFilter c)
                {
                    c.Value1 = null;
                    c.Value2 = null;
                }
            cmdRefresh_Executed(null, null);
        }

        /// <summary>
        /// Refresh
        /// </summary>
        internal async virtual void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            UpdateFilters();
            await Task.Run(() => {
                D.TotalItems = SQL.CountInstances(D.Module, D.FilterSqlString);
                D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.Module, D.FilterSqlString, D.FilterSqlParams, D.Sorting, 0);
            });
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Help
        /// </summary>
        internal void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e) => Functions.OpenHelp(this);

        /// <summary>
        /// Close
        /// </summary>
        internal void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e) => W.Close();

        /// <summary>
        /// Select
        /// </summary>
		internal MODULE_MODEL Selected;
        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (D.Mode != SE.Commands.Type.SELECT)
                {
                    if (Global.User.Perms.Contains($"{D.Module}_{Global.PermType.SAVE}"))
                        cmdEdit_Executed(null, null);
                    else
                        cmdPreview_Executed(null, null);
                }
                else cmdSelect_Executed(null, null);
            }
        }

        /// <summary>
        /// Load more
        /// </summary>
        internal void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                Cursor = Cursors.Wait;
                foreach (var i in SQL.ListInstances<MODULE_MODEL>(D.Module, D.FilterSqlString, D.FilterSqlParams, D.Sorting, (D.InstancesList as ObservableCollection<M>)?.Count ?? 0))
                    D.InstancesList.Add(i);
                (e.OriginalSource as ScrollViewer).ScrollToVerticalOffset(e.VerticalOffset);
                Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Sorting
        /// </summary>
        private void dgList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var sort = CollectionViewSource.GetDefaultView((sender as DataGrid).ItemsSource).SortDescriptions;
            if (sort.Any(x => x.PropertyName == e.Column.SortMemberPath))
            {
                D.Sorting[D.Sorting.IndexOf($"{Config.GetModuleAlias(D.Module)}.{e.Column.SortMemberPath.ToLower()}") + 1] = e.Column.SortDirection == ListSortDirection.Descending ? "desc" : "asc";
                return;
            }

            var limit = Convert.ToInt32(D.Sorting[4]);
            D.Sorting = new StringCollection
            {
                $"{Config.GetModuleAlias(D.Module)}.{e.Column.SortMemberPath.ToLower()}",
                e.Column.SortDirection == ListSortDirection.Descending ? "desc" : "asc"
            };
            if (sort.Count > 0)
            {
                D.Sorting.Add($"{Config.GetModuleAlias(D.Module)}.{sort[0].PropertyName.ToLower()}");
                D.Sorting.Add(sort[0].Direction == ListSortDirection.Descending ? "desc" : "asc");
            }
            else
            {
                D.Sorting.Add(D.Sorting[0]);
                D.Sorting.Add(D.Sorting[1]);
            }
            D.Sorting.Add(limit.ToString());
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            if (W.Owner != null)
                W.Owner.Focus();
        }
    }
}
