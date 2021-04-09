﻿using StswExpress.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
    class D_IconsNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = M_Module.Module.ATTRIBUTES_CLASSES;
        
        /// Window title
        public string Title
        {
            get
            {
                if      (Mode == Commands.Type.NEW)         return "Nowa klasa atrybutu";
                else if (Mode == Commands.Type.DUPLICATE)   return $"Duplikowanie klasy atrybutu: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)        return $"Edycja klasy atrybutu: {InstanceInfo.Name}";
                else                                        return $"Podgląd klasy atrybutu: {InstanceInfo.Name}";
            }
        }
    }
}
