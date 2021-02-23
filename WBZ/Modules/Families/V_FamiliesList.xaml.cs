using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyList.xaml
    /// </summary>
    public partial class FamiliesList : List
    {
        D_FamiliesList D = new D_FamiliesList();

        public FamiliesList(StswExpress.Globals.Commands.Type mode)
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
            D.FilterSQL = $"LOWER(COALESCE(f.declarant,'')) like '%{D.Filters.Declarant.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
                        + (D.Filters.Members > 0 ? $"COALESCE(f.members,0) = {D.Filters.Members} and " : string.Empty)
                        + $"LOWER(COALESCE(f.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"f.archival=false and " : string.Empty)
                        + (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=f.id and g.owner={D.Filters.Group}) and " : string.Empty);

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
