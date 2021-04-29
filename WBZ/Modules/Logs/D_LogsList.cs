using StswExpress.Globals;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Log;

namespace WBZ.Modules.Logs
{
    class D_LogsList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Config.Modules.LOGS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_LogsList;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista logów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór logu";
				else									return string.Empty;
			}
		}
		/// Instances list (logs)
		private ObservableCollection<MODULE_MODEL> instancesList_Logs = new ObservableCollection<MODULE_MODEL>();
		public ObservableCollection<MODULE_MODEL> InstancesList_Logs
		{
			get => instancesList_Logs;
			set => SetField(ref instancesList_Logs, value, () => InstancesList_Logs);
		}
		
		/// Instances list (errors)
		private ObservableCollection<MODULE_MODEL> instancesList_Errors = new ObservableCollection<MODULE_MODEL>();
		public ObservableCollection<MODULE_MODEL> InstancesList_Errors
		{
			get => instancesList_Errors;
			set => SetField(ref instancesList_Errors, value, () => InstancesList_Errors);
		}
	}
}
