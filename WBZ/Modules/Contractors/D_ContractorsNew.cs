using System.Collections.Generic;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Instance source - documents
		private List<M_Document> instanceSources_Documents;
        public List<M_Document> InstanceSources_Documents
        {
            get => instanceSources_Documents;
            set => SetField(ref instanceSources_Documents, value, () => InstanceSources_Documents);
        }
    }
}
