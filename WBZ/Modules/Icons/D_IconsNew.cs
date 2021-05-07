using StswExpress.Globals;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
    class D_IconsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string Module = Config.Modules.ICONS;
        
        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return $"Nowa ikona";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie ikony: {InstanceData.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja ikony: {InstanceData.Name}";
                else if (Mode == Commands.Type.PREVIEW)     return $"Podgląd ikony: {InstanceData.Name}";
                else                                        return string.Empty;
            }
        }
    }
}
