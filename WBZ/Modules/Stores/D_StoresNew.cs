using StswExpress.Globals;
using System.Collections.ObjectModel;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_TransportNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = M_Module.Module.STORES;

		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return "Nowy magazyn";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie magazynu: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja magazynu: {InstanceInfo.Name}";
				else										return $"Podgląd magazynu: {InstanceInfo.Name}";
			}
		}
		/// Instance source - articles
		private ObservableCollection<M_Article> instanceSources_Articles;
		public ObservableCollection<M_Article> InstanceSources_Articles
		{
			get => instanceSources_Articles;
			set
			{
				instanceSources_Articles = value;
				NotifyPropertyChanged();
			}
		}
		/// Instance source - documents
		private ObservableCollection<M_Document> instanceSources_Documents;
		public ObservableCollection<M_Document> InstanceSources_Documents
		{
			get => instanceSources_Documents;
			set
			{
				instanceSources_Documents = value;
				NotifyPropertyChanged();
			}
		}
	}
}
