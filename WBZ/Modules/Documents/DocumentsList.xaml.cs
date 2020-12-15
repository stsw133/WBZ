using WBZ.Globals;
using WBZ.Interfaces;
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
			cmdRefresh_Executed(null, null);

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
						+ $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.CompanyName.ToLower()}%' and "
						+ $"d.dateissue between '{D.Filters.fDateIssue:yyyy-MM-dd}' and '{D.Filters.DateIssue:yyyy-MM-dd} 23:59:59' and "
						+ (!D.Filters.Archival ? $"d.archival=false and " : "")
						+ (D.Filters.Group > 0 ? $"g.owner={D.Filters.Group} and " : "");

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
