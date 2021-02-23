using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    /// <summary>
    /// Logika interakcji dla klasy ContractorsList.xaml
    /// </summary>
    public partial class ContractorsList : List
    {
        D_ContractorsList D = new D_ContractorsList();

        public ContractorsList(StswExpress.Globals.Commands.Type mode)
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
            D.FilterSQL = $"LOWER(COALESCE(c.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.branch,'')) like '%{D.Filters.Branch.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.nip,'')) like '%{D.Filters.NIP.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.regon,'')) like '%{D.Filters.REGON.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(c.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"c.archival=false and " : string.Empty)
                        + (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=c.id and g.owner={D.Filters.Group}) and " : string.Empty);

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
