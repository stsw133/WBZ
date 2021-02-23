using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Group;

namespace WBZ.Modules.Groups
{
    /// <summary>
    /// Logika interakcji dla klasy GroupsNew.xaml
    /// </summary>
    public partial class GroupsNew : New
    {
        D_GroupsNew D = new D_GroupsNew();

        public GroupsNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
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
