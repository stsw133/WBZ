using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Log;

namespace WBZ.Modules.Logs
{
    class D_LogsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.LOGS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_LogsList;

		/// Instances list (logs)
		private ObservableCollection<MODULE_MODEL> instancesList_Logs = new ObservableCollection<MODULE_MODEL>();
		public ObservableCollection<MODULE_MODEL> InstancesList_Logs
		{
			get => instancesList_Logs;
			set
			{
				instancesList_Logs = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
		
		/// Instances list (errors)
		private ObservableCollection<MODULE_MODEL> instancesList_Errors = new ObservableCollection<MODULE_MODEL>();
		public ObservableCollection<MODULE_MODEL> InstancesList_Errors
		{
			get => instancesList_Errors;
			set
			{
				instancesList_Errors = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
	}
}
