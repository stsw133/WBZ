using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsList.xaml
	/// </summary>
	public partial class DocumentsList : List
	{
		D_DocumentsList D = new D_DocumentsList();

		public DocumentsList(Commands.Type mode)
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
			D.FilterSqlString = $"LOWER(COALESCE(d.type,'')) like '%{D.Filters.Type.ToLower()}%' and "
						+ $"LOWER(COALESCE(d.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.name,'')) like '%{D.Filters.cStore.Display?.ToString()?.ToLower()}%' and "
						+ $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.ContractorName.ToLower()}%' and "
						+ $"d.dateissue >= '{D.Filters.fDateIssue:yyyy-MM-dd}' and d.dateissue < '{D.Filters.DateIssue.AddDays(1):yyyy-MM-dd}' and "
						+ (!D.Filters.Archival ? $"d.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=d.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
