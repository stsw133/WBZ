using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;
using WBZ.Models;
using WBZ.Modules.Distributions;
using WBZ.Other;
using MODULE_CLASS = WBZ.Models.C_Family;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyNew.xaml
    /// </summary>
    public partial class FamiliesNew : Window
    {
        D_FamiliesNew D = new D_FamiliesNew();

        public FamiliesNew(MODULE_CLASS instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;

            D.InstanceInfo = instance;
            D.Mode = mode;

            if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
                D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
        }

        /// <summary>
		/// Validation
		/// </summary>
        private bool CheckDataValidation()
        {
            bool result = true;

            return result;
        }

        /// <summary>
		/// Save
		/// </summary>
        private bool saved = false;
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (!CheckDataValidation())
                return;

            if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
                Close();
        }

        /// <summary>
		/// Print
		/// </summary>
        private void btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn != null)
                btn.ContextMenu.IsOpen = true;
        }
        private void btnRodo_Click(object sender, RoutedEventArgs e)
        {
            Prints.Print_RODO(D.InstanceInfo, SQL.ListContacts(D.MODULE_NAME, D.InstanceInfo.ID, "default = true").DataTableToList<C_Contact>()?[0]);
        }

        /// <summary>
		/// Refresh
		/// </summary>
        private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            if (D.InstanceInfo.ID == 0)
                return;
            //TODO - dorobić odświeżanie zmienionych danych
        }

        /// <summary>
		/// Close
		/// </summary>
        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        /// <summary>
		/// Tab changed
		/// </summary>
        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name == "tabSources_Distributions")
            {
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Distributions == null)
                    D.InstanceSources_Distributions = SQL.ListInstances(Global.Module.DISTRIBUTIONS, $"dp.family={D.InstanceInfo.ID}").DataTableToList<C_Distribution>();
            }
        }

        /// <summary>
		/// Open: Distribution
		/// </summary>
        private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}")
                    ? Commands.Type.EDIT : Commands.Type.PREVIEW;

                var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Distribution>();
                foreach (C_Distribution instance in selectedInstances)
                {
                    var window = new DistributionsNew(instance, perm);
                    window.Show();
                }
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
                SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
        }
    }
}
