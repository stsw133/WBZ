using StswExpress.Globals;
using System.Collections.ObjectModel;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = M_Module.Module.CONTRACTORS;

        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return "Nowy kontrahent";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie kontrahenta: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja kontrahenta: {InstanceInfo.Name}";
                else                                        return $"Podgląd kontrahenta: {InstanceInfo.Name}";
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
