using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
	/// <summary>
	/// Interaction logic for IconsNew.xaml
	/// </summary>
	public partial class IconsNew : New
	{
        D_IconsNew D = new D_IconsNew();

        public IconsNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
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
