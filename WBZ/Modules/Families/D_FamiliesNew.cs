using System.Collections.Generic;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    class D_FamiliesNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Instance source - distributions
        private List<M_Distribution> instanceSources_Distributions;
        public List<M_Distribution> InstanceSources_Distributions
        {
            get => instanceSources_Distributions;
            set => SetField(ref instanceSources_Distributions, value, () => InstanceSources_Distributions);
        }
    }
}
