﻿using StswExpress.Globals;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._shared
{
    class D_GroupsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Config.Modules.GROUPS;

        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return $"Nowa grupa";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie grupy: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja grupy: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.PREVIEW)     return $"Podgląd grupy: {InstanceInfo.Name}";
                else                                        return string.Empty;
            }
        }
    }
}
