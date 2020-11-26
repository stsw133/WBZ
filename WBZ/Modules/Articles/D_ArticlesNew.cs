using System.Collections.Generic;
using System.Reflection;
using WBZ.Globals;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.C_Article;

namespace WBZ.Modules.Articles
{
    internal class D_ArticlesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.ARTICLES;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.NEW)
					return "Nowy towar";
				else if (Mode == Commands.Type.DUPLICATE)
					return $"Duplikowanie towaru: {InstanceInfo.Name}";
				else if (Mode == Commands.Type.EDIT)
					return $"Edycja towaru: {InstanceInfo.Name}";
				else
					return $"Podgląd towaru: {InstanceInfo.Name}";
			}
		}
		/// Instance source - stores
		private List<C_Store> instanceSources_Stores = new List<C_Store>();
		public List<C_Store> InstanceSources_Stores
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
		private List<C_Document> instanceSources_Documents = new List<C_Document>();
		public List<C_Document> InstanceSources_Documents
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
		private List<C_Distribution> instanceSources_Distributions = new List<C_Distribution>();
		public List<C_Distribution> InstanceSources_Distributions
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
