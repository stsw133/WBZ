using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresList.xaml
	/// </summary>
	public partial class StoresList : List
	{
		D_TransportList D = new D_TransportList();

		public StoresList(StswExpress.Globals.Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(s.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.city,'')) like '%{D.Filters.City.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"s.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=s.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
