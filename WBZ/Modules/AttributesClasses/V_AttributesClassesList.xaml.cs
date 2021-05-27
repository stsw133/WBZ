using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesList.xaml
	/// </summary>
	public partial class AttributesClassesList : List
	{
		D_IconsList D = new D_IconsList();

		public AttributesClassesList(Commands.Type mode)
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
			D.FilterSqlString = $"LOWER(COALESCE(ac.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(ac.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(ac.type,'')) like '%{D.Filters.Type.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"ac.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=ac.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
