using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
	/// <summary>
	/// Interaction logic for IconsList.xaml
	/// </summary>
	public partial class IconsList : List
	{
		D_IconsList D = new D_IconsList();

		public IconsList(Commands.Type mode)
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
			D.FilterSQL = $"(LOWER(COALESCE(i.module,'')) like '%{D.Filters.Module.ToLower()}%' or i.module='') and "
						+ $"LOWER(COALESCE(i.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"i.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=i.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
