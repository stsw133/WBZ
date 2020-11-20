using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.C_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesNew : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// Module
        public readonly string MODULE_TYPE = Global.Module.FAMILIES;
        /// Instance
		private MODULE_MODEL instanceInfo = new MODULE_MODEL();
        public MODULE_MODEL InstanceInfo
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
        /// Window mode
		public Commands.Type Mode { get; set; }
        /// Editing mode
        public bool EditingMode { get { return Mode != Commands.Type.PREVIEW; } }
        /// Additional window icon
        public string ModeIcon
        {
            get
            {
                if (Mode == Commands.Type.NEW)
                    return "/Resources/icon32_add.ico";
                else if (Mode == Commands.Type.DUPLICATE)
                    return "/Resources/icon32_duplicate.ico";
                else if (Mode == Commands.Type.EDIT)
                    return "/Resources/icon32_edit.ico";
                else
                    return "/Resources/icon32_search.ico";
            }
        }
        /// Window title
        public string Title
        {
            get
            {
                if (Mode == Commands.Type.NEW)
                    return "Nowa rodzina";
                else if (Mode == Commands.Type.DUPLICATE)
                    return $"Duplikowanie rodziny: {InstanceInfo.Lastname}";
                else if (Mode == Commands.Type.EDIT)
                    return $"Edycja rodziny: {InstanceInfo.Lastname}";
                else
                    return $"Podgląd rodziny: {InstanceInfo.Lastname}";
            }
        }
    }
}
