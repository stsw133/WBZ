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

		public DocumentsList(StswExpress.Globals.Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(d.type,'')) like '%{D.Filters.Type.ToLower()}%' and "
						+ $"LOWER(COALESCE(d.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(s.name,'')) like '%{D.Filters.StoreName.ToLower()}%' and "
						+ $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.ContractorName.ToLower()}%' and "
						+ $"d.dateissue >= '{D.Filters.fDateIssue:yyyy-MM-dd}' and d.dateissue <= '{D.Filters.DateIssue:yyyy-MM-dd}' and "
						+ (!D.Filters.Archival ? $"d.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=d.id and g.owner={D.Filters.Group}) and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
