using StswExpress.Globals;
using System.ComponentModel;
using System.Reflection;

namespace WBZ.Modules._base
{
    class D_ModuleNew<MODULE_MODEL> : INotifyPropertyChanged where MODULE_MODEL : class, new()
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Instance
		private MODULE_MODEL instanceInfo = new MODULE_MODEL();
		public MODULE_MODEL InstanceInfo
		{
			get => instanceInfo;
			set
			{
				instanceInfo = value;
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
	}
}
