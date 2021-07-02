using System.Collections.Generic;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    internal class D_ArticlesNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Instance source - distributions
        private List<M_Distribution> instanceSources_Distributions;
        public List<M_Distribution> InstanceSources_Distributions
        {
            get => instanceSources_Distributions;
            set => SetField(ref instanceSources_Distributions, value, () => InstanceSources_Distributions);
        }

        /// Instance source - documents
        private List<M_Document> instanceSources_Documents;
        public List<M_Document> InstanceSources_Documents
        {
            get => instanceSources_Documents;
            set => SetField(ref instanceSources_Documents, value, () => InstanceSources_Documents);
        }

        /// Instance source - stores
        private List<M_Store> instanceSources_Stores;
        public List<M_Store> InstanceSources_Stores
        {
            get => instanceSources_Stores;
            set => SetField(ref instanceSources_Stores, value, () => InstanceSources_Stores);
        }
    }
}
