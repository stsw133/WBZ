using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
	/// <summary>
	/// Interaction logic for VehiclesList.xaml
	/// </summary>
	public partial class VehiclesList : List
	{
		D_VehiclesList D = new D_VehiclesList();

		public VehiclesList(StswExpress.Globals.Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(v.register,'')) like '%{D.Filters.Register.ToLower()}%' and "
						+ $"LOWER(COALESCE(v.brand,'')) like '%{D.Filters.Brand.ToLower()}%' and "
						+ $"LOWER(COALESCE(v.model,'')) like '%{D.Filters.Model.ToLower()}%' and "
						+ (D.Filters.Capacity > 0 ? $"COALESCE(v.capacity,0) = {D.Filters.Capacity} and " : string.Empty)
						//+ $"LOWER(COALESCE(v.forwarder,'')) like '%{D.Filters.Forwarder.Codename.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"v.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=v.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
