using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    /// <summary>
    /// Interaction logic for UsersList.xaml
    /// </summary>
    public partial class UsersList : List
    {
        readonly D_UsersList D = new D_UsersList();

        public UsersList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            D.Mode = mode;
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
