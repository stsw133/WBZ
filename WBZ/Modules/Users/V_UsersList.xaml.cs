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
			base.Init();

			D.Mode = mode;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public override void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(u.forename,'')) LIKE '%{D.Filters.Forename.ToLower()}%' AND "
						+ $"LOWER(COALESCE(u.lastname,'')) LIKE '%{D.Filters.Lastname.ToLower()}%' AND "
						+ $"LOWER(COALESCE(u.email,'')) LIKE '%{D.Filters.Email.ToLower()}%' AND "
						+ $"LOWER(COALESCE(u.phone,'')) LIKE '%{D.Filters.Phone.ToLower()}%' AND "
						+ (!D.Filters.Archival ? $"u.archival=false AND " : string.Empty)
						+ (D.Filters.Group > 0 ? $"EXISTS (SELECT FROM wbz.groups g WHERE g.instance=u.id AND g.owner={D.Filters.Group}) AND " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" AND ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
