using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesNew.xaml
	/// </summary>
	public partial class AttributesClassesNew : New
	{
        D_AttributesClassesNew D = new D_AttributesClassesNew();

        public AttributesClassesNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
