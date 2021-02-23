using System.Collections.ObjectModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    class D_ArticlesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.ARTICLES;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == StswExpress.Globals.Commands.Type.NEW)
					return "Nowy towar";
				else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
					return $"Duplikowanie towaru: {InstanceInfo.Name}";
				else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
					return $"Edycja towaru: {InstanceInfo.Name}";
				else
					return $"Podgląd towaru: {InstanceInfo.Name}";
			}
		}
		/// Instance source - stores
		private ObservableCollection<M_Store> instanceSources_Stores;
		public ObservableCollection<M_Store> InstanceSources_Stores
		{
			get
			{
				return instanceSources_Stores;
			}
			set
			{
				instanceSources_Stores = value;
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
		/// Instance source - distributions
		private ObservableCollection<M_Distribution> instanceSources_Distributions;
		public ObservableCollection<M_Distribution> InstanceSources_Distributions
		{
			get
			{
				return instanceSources_Distributions;
			}
			set
			{
				instanceSources_Distributions = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
	}
}
