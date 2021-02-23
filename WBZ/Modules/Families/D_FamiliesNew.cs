using System.Collections.ObjectModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.FAMILIES;

        /// Window title
        public string Title
        {
            get
            {
                if (Mode == StswExpress.Globals.Commands.Type.NEW)
                    return "Nowa rodzina";
                else if (Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
                    return $"Duplikowanie rodziny: {InstanceInfo.Lastname}";
                else if (Mode == StswExpress.Globals.Commands.Type.EDIT)
                    return $"Edycja rodziny: {InstanceInfo.Lastname}";
                else
                    return $"Podgląd rodziny: {InstanceInfo.Lastname}";
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
