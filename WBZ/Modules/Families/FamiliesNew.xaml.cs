using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Distributions;
using MODULE_CLASS = WBZ.Classes.C_Family;
using WBZ.Other;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyNew.xaml
    /// </summary>
    public partial class FamiliesNew : Window
    {
        M_FamiliesNew M = new M_FamiliesNew();

        public FamiliesNew(MODULE_CLASS instance, Global.ActionType mode)
        {
            InitializeComponent();
            DataContext = M;

            M.InstanceInfo = instance;
            M.Mode = mode;

            if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
                M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
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

            if (saved = SQL.SetInstance(M.MODULE_NAME, M.InstanceInfo, M.Mode))
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
            Prints.Print_RODO(M.InstanceInfo, SQL.ListContacts(M.MODULE_NAME, M.InstanceInfo.ID, "default = true").DataTableToList<C_Contact>()?[0]);
        }

        /// <summary>
		/// Refresh
		/// </summary>
        private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            if (M.InstanceInfo.ID == 0)
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
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Distributions == null)
                    M.InstanceSources_Distributions = SQL.ListInstances(Global.Module.DISTRIBUTIONS, $"dp.family={M.InstanceInfo.ID}").DataTableToList<C_Distribution>();
            }
        }

        /// <summary>
		/// Open: Distribution
		/// </summary>
        private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}")
                    ? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

                var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Distribution>();
                foreach (C_Distribution instance in selectedInstances)
                {
                    var window = new DistributionsAdd(instance, false/*perm*/);
                    window.Show();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
                SQL.ClearObject(M.MODULE_NAME, M.InstanceInfo.ID);
        }
    }

    /// <summary>
	/// Model
	/// </summary>
	internal class M_FamiliesNew : INotifyPropertyChanged
    {
        public readonly string MODULE_NAME = Global.Module.FAMILIES;

        /// Logged user
        public C_User User { get; } = Global.User;
        /// Instance
		private MODULE_CLASS instanceInfo;
        public MODULE_CLASS InstanceInfo
        {
            get
            {
                return instanceInfo;
            }
            set
            {
                instanceInfo = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Instance source - distributions
        private List<C_Distribution> instanceSources_Distributions;
        public List<C_Distribution> InstanceSources_Distributions
        {
            get
            {
                return instanceSources_Distributions;
            }
            set
            {
                instanceSources_Distributions = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Editing mode
        public bool EditingMode { get { return Mode != Global.ActionType.PREVIEW; } }
        /// Tryb okna
        public Global.ActionType Mode { get; set; }
        /// Dodatkowa ikona okna
        public string ModeIcon
        {
            get
            {
                if (Mode == Global.ActionType.NEW)
                    return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
                else if (Mode == Global.ActionType.DUPLICATE)
                    return "pack://siteoforigin:,,,/Resources/icon32_duplicate.ico";
                else if (Mode == Global.ActionType.EDIT)
                    return "pack://siteoforigin:,,,/Resources/icon32_edit.ico";
                else
                    return "pack://siteoforigin:,,,/Resources/icon32_search.ico";
            }
        }
        /// Tytuł okna
        public string Title
        {
            get
            {
                if (Mode == Global.ActionType.NEW)
                    return "Nowa rodzina";
                else if (Mode == Global.ActionType.DUPLICATE)
                    return $"Duplikowanie rodziny: {InstanceInfo.Lastname}";
                else if (Mode == Global.ActionType.EDIT)
                    return $"Edycja rodziny: {InstanceInfo.Lastname}";
                else
                    return $"Podgląd rodziny: {InstanceInfo.Lastname}";
            }
        }

        /// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
