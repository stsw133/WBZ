using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_Company;

namespace WBZ.Modules.Companies
{
    class D_CompaniesList : INotifyPropertyChanged
    {
        public readonly string MODULE_NAME = Global.Module.COMPANIES;
        public StringCollection SORTING = Properties.Settings.Default.sorting_CompaniesList;

        /// Instances list
		private List<MODULE_CLASS> instancesList;
        public List<MODULE_CLASS> InstancesList
        {
            get
            {
                return instancesList;
            }
            set
            {
                instancesList = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Selecting mode
        public bool SelectingMode { get; set; }
        /// SQL filter
        public string FilterSQL { get; set; }
        /// Filter instance
        private MODULE_CLASS filters = new MODULE_CLASS();
        public MODULE_CLASS Filters
        {
            get
            {
                return filters;
            }
            set
            {
                filters = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Page number
        private int page;
        public int Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Total instances number
        private int totalItems;
        public int TotalItems
        {
            get
            {
                return totalItems;
            }
            set
            {
                totalItems = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
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
