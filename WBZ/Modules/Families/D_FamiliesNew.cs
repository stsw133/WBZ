using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using WBZ.Models;
using MODULE_CLASS = WBZ.Models.C_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesNew : INotifyPropertyChanged
    {
        public readonly string MODULE_NAME = Global.Module.FAMILIES;

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
        /// Window mode
		public Global.ActionType Mode { get; set; }
        /// Additional window icon
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
        /// Window title
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
