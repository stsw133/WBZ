using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_Employee;

namespace WBZ.Modules.Employees
{
    class D_EmployeesNew : INotifyPropertyChanged
    {
		public readonly string MODULE_NAME = Global.Module.EMPLOYEES;

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
					return "Nowy pracownik";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie pracownika: {InstanceInfo.Fullname}";
				else if (Mode == Commands.Type.EDIT)
					return $"Edycja pracownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd pracownika: {InstanceInfo.Fullname}";
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
