using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesList.xaml
    /// </summary>
    public partial class EmployeesList : List
    {
        readonly D_EmployeesList D = new D_EmployeesList();

        public EmployeesList(Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(e.forename,'')) LIKE '%{D.Filters.Forename.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.lastname,'')) LIKE '%{D.Filters.Lastname.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.department,'')) LIKE '%{D.Filters.Department.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.position,'')) LIKE '%{D.Filters.Position.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.email,'')) LIKE '%{D.Filters.Email.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.phone,'')) LIKE '%{D.Filters.Phone.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.postcode,'')) LIKE '%{D.Filters.Postcode.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.city,'')) LIKE '%{D.Filters.City.ToLower()}%' AND "
                        + $"LOWER(COALESCE(e.address,'')) LIKE '%{D.Filters.Address.ToLower()}%' AND "
                        + (!D.Filters.Archival ? $"e.archival=false AND " : string.Empty)
                        + (D.Filters.Group > 0 ? $"EXISTS (SELECT FROM wbz.groups g WHERE g.instance=e.id AND g.owner={D.Filters.Group}) AND " : string.Empty);

            D.FilterSQL = D.FilterSQL.TrimEnd(" AND ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
