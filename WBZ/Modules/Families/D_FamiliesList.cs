using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using MODULE_MODEL = WBZ.Models.C_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// Module
        public readonly string MODULE_TYPE = Global.Module.FAMILIES;
        public StringCollection SORTING = Properties.Settings.Default.sorting_FamiliesList;
        /// Instances list
		private List<MODULE_MODEL> instancesList;
        public List<MODULE_MODEL> InstancesList
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
        /// Mode
		public Commands.Type Mode { get; set; }
        /// Selecting mode
        public bool SelectingMode { get { return Mode == Commands.Type.SELECTING; } }
        /// SQL filter
        public string FilterSQL { get; set; }
        /// Filter instance
        private MODULE_MODEL filters = new MODULE_MODEL();
        public MODULE_MODEL Filters
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
    }
}
