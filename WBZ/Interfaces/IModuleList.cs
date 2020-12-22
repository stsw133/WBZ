using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;

namespace WBZ.Interfaces
{
    interface IModuleList
	{
        void Init();
        void Window_Loaded(object sender, RoutedEventArgs e);
        void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdNew_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdDuplicate_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdDelete_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdClear_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e);
        void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e);
        void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e);
        void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e);
        void Window_Closed(object sender, EventArgs e);
    }

    public class ModuleList<MODULE_MODEL> : Window, IModuleList where MODULE_MODEL : class, new()
    {
        dynamic W, D;
        string FullName, HalfName;

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            W = GetWindow(this);
            D = W.DataContext;
            FullName = W.GetType().FullName;
            HalfName = FullName.Substring(0, FullName.Length - 4);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D.SelectingMode)
                W.dgList.SelectionMode = DataGridSelectionMode.Single;
        }

        /// <summary>
        /// Preview
        /// </summary>
        internal void cmdPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.PREVIEW}"))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch { }
        }
        public void cmdPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = (W.dgList as DataGrid).SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.PREVIEW) as Window;
                window.Show();
            }
        }

        /// <summary>
		/// New
		/// </summary>
		internal void cmdNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.SAVE}"))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch { }
        }
        public void cmdNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), null, Commands.Type.NEW) as Window;
            window.Show();
        }

        /// <summary>
        /// Duplicate
        /// </summary>
        internal void cmdDuplicate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.SAVE}"))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch { }
        }
        public void cmdDuplicate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = (W.dgList as DataGrid).SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.DUPLICATE) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        internal void cmdEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.SAVE}"))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch { }
        }
        public void cmdEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = (W.dgList as DataGrid).SelectedItems.Cast<MODULE_MODEL>();
            foreach (MODULE_MODEL instance in selectedInstances)
            {
                var window = Activator.CreateInstance(Type.GetType(HalfName + "New"), instance, Commands.Type.EDIT) as Window;
                window.Show();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        internal void cmdDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.DELETE}"))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch { }
        }
        public void cmdDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedInstances = (W.dgList as DataGrid).SelectedItems.Cast<MODULE_MODEL>();
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
		public void cmdClear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            D.Filters = new MODULE_MODEL();
            cmdRefresh_Executed(null, null);
        }

        /// <summary>
        /// Refresh
        /// </summary>
        public async void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await Task.Run(() => {
                W.UpdateFilters();
                D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL);
                D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, D.Page = 0);
            });
        }

        /// <summary>
        /// Help
        /// </summary>
        public void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Functions.OpenHelp(this);
        }

        /// <summary>
        /// Close
        /// </summary>
        public void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            W.Close();
        }

        /// <summary>
        /// Select
        /// </summary>
		public MODULE_MODEL Selected;
        public void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!D.SelectingMode)
                {
                    if (Global.User.Perms.Contains($"{D.MODULE_TYPE}_{Global.UserPermType.SAVE}"))
                        cmdEdit_Executed(null, null);
                    else
                        cmdPreview_Executed(null, null);
                }
                else
                {
                    W.Selected = W.dgList.SelectedItems.Cast<MODULE_MODEL>().FirstOrDefault();
                    W.DialogResult = true;
                }
            }
        }

        /// <summary>
        /// Load more
        /// </summary>
        public void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && D.InstancesList.Count < D.TotalItems)
            {
                W.DataContext = null;
                D.InstancesList.AddRange(SQL.ListInstances<MODULE_MODEL>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, ++D.Page));
                W.DataContext = D;
                Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        public void Window_Closed(object sender, EventArgs e)
        {
            if (W.Owner != null)
                W.Owner.Focus();
        }
    }
}
