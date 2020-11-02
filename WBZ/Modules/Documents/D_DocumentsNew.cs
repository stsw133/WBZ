using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using WBZ.Models;
using MODULE_CLASS = WBZ.Models.C_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsNew
    {
		public readonly string MODULE_NAME = Global.Module.DOCUMENTS;

		/// Logged user
		public C_User User { get; } = Global.User;
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
					return "Nowy dokument";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == Global.ActionType.EDIT)
					return $"Edycja dokumentu: {InstanceInfo.Name}";
				else
					return $"Podgląd dokumentu: {InstanceInfo.Name}";
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
