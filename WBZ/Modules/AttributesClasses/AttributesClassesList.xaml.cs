using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesList.xaml
	/// </summary>
	public partial class AttributesClassesList : List
	{
		D_AttributesClassesList D = new D_AttributesClassesList();

		public AttributesClassesList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();
			cmdRefresh_Executed(null, null);

			D.Mode = mode;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(ac.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(ac.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(ac.type,'')) like '%{D.Filters.Type.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"ac.archival=false and " : "")
						+ (D.Filters.Group > 0 ? $"g.owner={D.Filters.Group} and " : "");

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
