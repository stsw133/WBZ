using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using MODULE_CLASS = WBZ.Models.C_Document;

namespace WBZ.Modules.Documents
{
    class D_DocumentsNew
    {
		public readonly string MODULE_NAME = Global.Module.DOCUMENTS;

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
					return "Nowy dokument";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)
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
