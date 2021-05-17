using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Vehicle;

namespace WBZ.Modules.Vehicles
{
	/// <summary>
	/// Interaction logic for VehiclesList.xaml
	/// </summary>
	public partial class VehiclesList : List
	{
        readonly D_VehiclesList D = new D_VehiclesList();

		public VehiclesList(Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(v.register,'')) LIKE '%{D.Filters.Register.ToLower()}%' AND "
						+ $"LOWER(COALESCE(v.brand,'')) LIKE '%{D.Filters.Brand.ToLower()}%' AND "
						+ $"LOWER(COALESCE(v.model,'')) LIKE '%{D.Filters.Model.ToLower()}%' AND "
						+ (D.Filters.Capacity > 0 ? $"COALESCE(v.capacity,0) = {D.Filters.Capacity} AND " : string.Empty)
						+ $"LOWER(COALESCE(c.name,'')) LIKE '%{D.Filters.ForwarderName.ToLower()}%' AND "
						+ $"LOWER(COALESCE(e.lastname || ' ' || e.forename,'')) LIKE '%{D.Filters.DriverName.ToLower()}%' AND "
						+ (D.Filters.ProdYear > 0 ? $"COALESCE(v.prodyear,0) = {D.Filters.ProdYear} AND " : string.Empty)
						+ (!D.Filters.Archival ? $"v.archival=false AND " : string.Empty)
						+ (D.Filters.Group > 0 ? $"EXISTS (SELECT FROM wbz.groups g WHERE g.instance=v.id AND g.owner={D.Filters.Group}) AND " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" AND ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
