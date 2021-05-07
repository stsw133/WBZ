using StswExpress.Globals;
using System.Collections.ObjectModel;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string Module = Config.Modules.FAMILIES;

        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return $"Nowa rodzina";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie rodziny: {InstanceData.Lastname}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja rodziny: {InstanceData.Lastname}";
                else if (Mode == Commands.Type.PREVIEW)     return $"Podgląd rodziny: {InstanceData.Lastname}";
                else                                        return string.Empty;
            }
        }
        /// Instance source - distributions
        private ObservableCollection<M_Distribution> instanceSources_Distributions;
        public ObservableCollection<M_Distribution> InstanceSources_Distributions
        {
            get => instanceSources_Distributions;
            set => SetField(ref instanceSources_Distributions, value, () => InstanceSources_Distributions);
        }
    }
}
