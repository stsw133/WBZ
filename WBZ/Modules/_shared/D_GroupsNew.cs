using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    class D_GroupsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return $"Nowa grupa";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie grupy: {InstanceData.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja grupy: {InstanceData.Name}";
                else if (Mode == Commands.Type.PREVIEW)     return $"Podgląd grupy: {InstanceData.Name}";
                else                                        return string.Empty;
            }
        }
    }
}
