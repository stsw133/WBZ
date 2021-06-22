using System.Collections.Generic;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
    class D_StoresNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Instance source - articles
		private List<M_Article> instanceSources_Articles;
		public List<M_Article> InstanceSources_Articles
		{
			get => instanceSources_Articles;
			set => SetField(ref instanceSources_Articles, value, () => InstanceSources_Articles);
		}

		/// Instance source - documents
		private List<M_Document> instanceSources_Documents;
		public List<M_Document> InstanceSources_Documents
		{
			get => instanceSources_Documents;
			set => SetField(ref instanceSources_Documents, value, () => InstanceSources_Documents);
		}
	}
}
