using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
	/// <summary>
	/// Interaction logic for UsersList.xaml
	/// </summary>
	public partial class UsersList : List
	{
		D_UsersList D = new D_UsersList();

		public UsersList(StswExpress.Globals.Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(u.forename,'')) like '%{D.Filters.Forename.ToLower()}%' and "
						+ $"LOWER(COALESCE(u.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
						+ $"LOWER(COALESCE(u.email,'')) like '%{D.Filters.Email.ToLower()}%' and "
						+ $"LOWER(COALESCE(u.phone,'')) like '%{D.Filters.Phone.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"u.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=u.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
