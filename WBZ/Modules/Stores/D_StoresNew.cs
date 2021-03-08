using System.Collections.ObjectModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_TransportNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.STORES;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowy magazyn";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie magazynu: {InstanceInfo.Name}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja magazynu: {InstanceInfo.Name}";
				else
					return $"Podgląd magazynu: {InstanceInfo.Name}";
			}
		}
		/// Instance source - articles
		private ObservableCollection<M_Article> instanceSources_Articles;
		public ObservableCollection<M_Article> InstanceSources_Articles
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
		private ObservableCollection<M_Document> instanceSources_Documents;
		public ObservableCollection<M_Document> InstanceSources_Documents
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
	}
}
