﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WBZ.Globals;

namespace WBZ.Modules._base
{
    public abstract class ModuleList<MODULE_MODEL> : Window where MODULE_MODEL : class, new()
    {
        dynamic W, D;
        DataGrid dgList;
        string FullName, HalfName;

        /// <summary>
        /// Init
        /// </summary>
        internal void Init()
        {
            W = GetWindow(this);
            D = W.DataContext;
            FullName = W.GetType().FullName;
            HalfName = FullName[0..^4];
            dgList = W.dgList;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        internal virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D.Mode == StswExpress.Globals.Commands.Type.SELECT)
                dgList.SelectionMode = DataGridSelectionMode.Single;
            cmdRefresh_Executed(null, null);
        }

        /// <summary>
		/// Update filters
		/// </summary>
		public virtual void UpdateFilters()
        { }

        /// <summary>
		/// Select
		/// </summary>
		internal void cmdSelect_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = D?.Mode == StswExpress.Globals.Commands.Type.SELECT ?? false;
        internal virtual void cmdSelect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Selected = dgList.SelectedItems.Cast<MODULE_MODEL>().FirstOrDefault();
            W.DialogResult = true;
        }

        /// <summary>
        /// Preview
        /// </summary>
        internal void cmdPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.MODULE_TYPE}_{Global.PermType.PREVIEW}");
        internal virtual void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, StswExpress.Globals.Commands.Type.PREVIEW) as Window;
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		internal void cmdNew_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.MODULE_TYPE}_{Global.PermType.SAVE}");
        internal virtual void cmdNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), null, StswExpress.Globals.Commands.Type.NEW) as Window;
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        internal void cmdDuplicate_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.MODULE_TYPE}_{Global.PermType.SAVE}");
        internal virtual void cmdDuplicate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, StswExpress.Globals.Commands.Type.DUPLICATE) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        internal void cmdEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.MODULE_TYPE}_{Global.PermType.SAVE}");
        internal virtual void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, StswExpress.Globals.Commands.Type.EDIT) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        internal void cmdDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Global.User.Perms.Contains($"{D?.MODULE_TYPE}_{Global.PermType.DELETE}");
        internal virtual void cmdDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = dgList.SelectedItems.Cast<MODULE_MODEL>();
            if (selectedInstances.Count() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (dynamic instance in selectedInstances)
                    SQL.DeleteInstance(D.MODULE_TYPE, instance.ID, instance.Name);
                cmdRefresh_Executed(null, null);
            }
        }

        /// <summary>
		/// Clear
		/// </summary>
		internal virtual void cmdClear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            D.Filters = new MODULE_MODEL();
            cmdRefresh_Executed(null, null);
        }

        /// <summary>
        /// Refresh
        /// </summary>
        internal async virtual void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            await Task.Run(() => {
                UpdateFilters();
                D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL);
                D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, D.Page = 0);
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
                if (D.Mode != StswExpress.Globals.Commands.Type.SELECT)
                {
                    if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.PermType.SAVE}"))
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
                foreach (var i in SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, ++D.Page))
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
            var sort = CollectionViewSource.GetDefaultView(dgList.ItemsSource).SortDescriptions;
            if (sort.Any(x => x.PropertyName == e.Column.SortMemberPath))
            {
                D.SORTING[D.SORTING.IndexOf($"{Config.GetModuleAlias(D.MODULE_TYPE)}.{e.Column.SortMemberPath.ToLower()}") + 1] = (e.Column.SortDirection == ListSortDirection.Descending).ToString();
                return;
            }

            var limit = Convert.ToInt32(D.SORTING?[4] ?? 50);
            D.SORTING = new StringCollection();
            D.SORTING.Add($"{Config.GetModuleAlias(D.MODULE_TYPE)}.{e.Column.SortMemberPath.ToLower()}");
            D.SORTING.Add((e.Column.SortDirection == ListSortDirection.Descending).ToString());
            if (sort.Count > 0)
            {
                D.SORTING.Add($"{Config.GetModuleAlias(D.MODULE_TYPE)}.{sort[0].PropertyName.ToLower()}");
                D.SORTING.Add((sort[0].Direction == ListSortDirection.Descending).ToString());
            }
            else
            {
                D.SORTING.Add(D.SORTING[0]);
                D.SORTING.Add(D.SORTING[1]);
            }
            D.SORTING.Add(limit.ToString());
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (W.Owner != null)
                W.Owner.Focus();
        }
    }
}
