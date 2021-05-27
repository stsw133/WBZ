﻿using StswExpress;
using WBZ.Modules._base;
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
		internal override void UpdateFilters()
		{
			D.FilterSqlString = $"LOWER(COALESCE(d.name,'')) like '%{D.Filters.Name}%' and "
						+ $"d.datereal >= '{D.Filters.fDateReal:yyyy-MM-dd}' and d.datereal < '{D.Filters.DateReal.AddDays(1):yyyy-MM-dd}' and "
						//+ (D.Filters.FamiliesCount > 0 ? $"COALESCE(count(family),0) = {M.Filters.FamiliesCount} and " : string.Empty)
						+ (!D.Filters.Archival ? $"d.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=d.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
