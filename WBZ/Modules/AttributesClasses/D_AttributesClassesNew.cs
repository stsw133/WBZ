using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
    class D_AttributesClassesNew : D_ModuleNew<MODULE_MODEL>
    {
        /// Module
        public readonly string MODULE_TYPE = Global.Module.ATTRIBUTES_CLASSES;
        
        /// Window title
        public string Title
        {
            get
            {
                if (Mode == Commands.Type.NEW)
                    return "Nowa klasa atrybutu";
                else if (Mode == Commands.Type.DUPLICATE)
                    return $"Duplikowanie klasy atrybutu: {InstanceInfo.Name}";
                else if (Mode == Commands.Type.EDIT)
                    return $"Edycja klasy atrybutu: {InstanceInfo.Name}";
                else
                    return $"Podgląd klasy atrybutu: {InstanceInfo.Name}";
            }
        }
    }
}
