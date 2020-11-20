using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_User;

namespace WBZ.Modules.Users
{
	/// <summary>
	/// Interaction logic for UsersList.xaml
	/// </summary>
	public partial class UsersList : List
	{
		D_UsersList D = new D_UsersList();

		public UsersList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();
			btnRefresh_Click(null, null);

			D.Mode = mode;
		}

		/// Selected
		public MODULE_MODEL Selected;
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
