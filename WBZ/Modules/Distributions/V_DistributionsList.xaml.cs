using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsList.xaml
	/// </summary>
	public partial class DistributionsList : List
	{
		D_DistributionsList D = new D_DistributionsList();

		public DistributionsList(Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(d.name,'')) like '%{D.Filters.Name}%' and "
						+ $"d.datereal between '{D.Filters.fDateReal:yyyy-MM-dd}' and '{D.Filters.DateReal:yyyy-MM-dd} 23:59:59' and "
						//+ (D.Filters.FamiliesCount > 0 ? $"COALESCE(count(family),0) = {M.Filters.FamiliesCount} and " : "")
						+ (!D.Filters.Archival ? $"d.archival=false and " : "")
						+ (D.Filters.Group > 0 ? $"g.owner={D.Filters.Group} and " : "");

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
