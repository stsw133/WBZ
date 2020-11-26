using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_StoresNew : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Module
		public readonly string MODULE_TYPE = Global.Module.STORES;
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
		/// Instance source - articles
		private List<M_Article> instanceSources_Articles;
		public List<M_Article> InstanceSources_Articles
		{
			get
			{
				return instanceSources_Articles;
			}
			set
			{
				instanceSources_Articles = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Instance source - documents
		private List<M_Document> instanceSources_Documents;
		public List<M_Document> InstanceSources_Documents
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
					return "Nowy magazyn";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie magazynu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)
					return $"Edycja magazynu: {InstanceInfo.Name}";
				else
					return $"Podgląd magazynu: {InstanceInfo.Name}";
			}
		}
	}
}
