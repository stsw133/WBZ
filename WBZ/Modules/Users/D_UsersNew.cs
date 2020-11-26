using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    class D_UsersNew : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Module
		public readonly string MODULE_TYPE = Global.Module.USERS;
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
		/// Mode
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
					return "Nowy użytkownik";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie użytkownika: {InstanceInfo.Fullname}";
				else if (Mode == Commands.Type.EDIT)
					return $"Edycja użytkownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd użytkownika: {InstanceInfo.Fullname}";
			}
		}
	}
}
