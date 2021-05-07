using StswExpress.Globals;
using System.Collections.ObjectModel;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string Module = Config.Modules.CONTRACTORS;

        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return $"Nowy kontrahent";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie kontrahenta: {InstanceData.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja kontrahenta: {InstanceData.Name}";
                else if (Mode == Commands.Type.PREVIEW)     return $"Podgląd kontrahenta: {InstanceData.Name}";
                else                                        return string.Empty;
            }
        }
        /// Instance source - documents
		private ObservableCollection<M_Document> instanceSources_Documents;
        public ObservableCollection<M_Document> InstanceSources_Documents
        {
            get => instanceSources_Documents;
            set => SetField(ref instanceSources_Documents, value, () => InstanceSources_Documents);
        }
    }
}
