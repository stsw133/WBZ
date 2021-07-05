using StswExpress;
using WBZ.Models;
using WBZ.Modules._base;
using WBZ.Modules._submodules.Groups;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules._submodules
{
    /// <summary>
    /// Logika interakcji dla klasy GroupsNew.xaml
    /// </summary>
    public partial class GroupsNew : New
    {
        readonly D_GroupsNew D = new D_GroupsNew();

        public GroupsNew(MODULE_MODEL instance, Commands.Type mode, MV module)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
            {
                D.InstanceData = instance;
                D.InstanceData.Module = module;
            }
            D.Mode = mode;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
