﻿using StswExpress.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
    class D_IconsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = M_Module.Module.ICONS;
        
        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return "Nowa ikona";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie ikony: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja ikony: {InstanceInfo.Name}";
                else                                        return $"Podgląd ikony: {InstanceInfo.Name}";
            }
        }
    }
}