using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Employee;

namespace WBZ.Modules.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesList.xaml
    /// </summary>
    public partial class EmployeesList : List
    {
        D_EmployeesList D = new D_EmployeesList();

        public EmployeesList(StswExpress.Globals.Commands.Type mode)
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
			D.FilterSQL = $"LOWER(COALESCE(e.forename,'')) like '%{D.Filters.Forename.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.department,'')) like '%{D.Filters.Department.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.position,'')) like '%{D.Filters.Position.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.email,'')) like '%{D.Filters.Email.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.phone,'')) like '%{D.Filters.Phone.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(e.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"e.archival=false and " : string.Empty)
                        + (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=e.id and g.owner={D.Filters.Group}) and " : string.Empty);

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
