using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using WBZ.Models;
using MODULE_CLASS = WBZ.Models.C_Company;

namespace WBZ.Modules.Companies
{
    class D_CompaniesNew : INotifyPropertyChanged
    {
        public readonly string MODULE_NAME = Global.Module.COMPANIES;

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
        /// Instance source - documents
		private List<C_Document> instanceSources_Documents;
        public List<C_Document> InstanceSources_Documents
        {
            get
            {
                return instanceSources_Documents;
            }
            set
            {
                instanceSources_Documents = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// Editing mode
		public bool EditingMode { get { return Mode != Commands.Type.PREVIEW; } }
        /// Window mode
		public Commands.Type Mode { get; set; }
        /// Additional window icon
        public string ModeIcon
        {
            get
            {
                if (Mode == Commands.Type.NEW)
                    return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
                else if (Mode == Commands.Type.DUPLICATE)
                    return "pack://siteoforigin:,,,/Resources/icon32_duplicate.ico";
                else if (Mode == Commands.Type.EDIT)
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
                if (Mode == Commands.Type.NEW)
                    return "Nowa firma";
                else if (Mode == Commands.Type.DUPLICATE)
                    return $"Duplikowanie firmy: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)
                    return $"Edycja firmy: {InstanceInfo.Name}";
                else
                    return $"Podgląd firmy: {InstanceInfo.Name}";
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
