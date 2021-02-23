using System.Collections.ObjectModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    class D_ContractorsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.CONTRACTORS;

        /// Window title
        public string Title
        {
            get
            {
                if (Mode == StswExpress.Globals.Commands.Type.NEW)
                    return "Nowy kontrahent";
                else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
                    return $"Duplikowanie kontrahenta: {InstanceInfo.Name}";
                else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
                    return $"Edycja kontrahenta: {InstanceInfo.Name}";
                else
                    return $"Podgląd kontrahenta: {InstanceInfo.Name}";
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
