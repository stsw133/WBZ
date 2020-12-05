using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyList.xaml
    /// </summary>
    public partial class FamiliesList : List
    {
        D_FamiliesList D = new D_FamiliesList();

        public FamiliesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();
            btnRefresh_Click(null, null);

            D.Mode = mode;
        }

        /// <summary>
        /// Update filters
        /// </summary>
        public void UpdateFilters()
        {
            D.FilterSQL = $"LOWER(COALESCE(f.declarant,'')) like '%{D.Filters.Declarant.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.lastname,'')) like '%{D.Filters.Lastname.ToLower()}%' and "
                        + (D.Filters.Members > 0 ? $"COALESCE(f.members,0) = {D.Filters.Members} and " : "")
                        + $"LOWER(COALESCE(f.postcode,'')) like '%{D.Filters.Postcode.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.city,'')) like '%{D.Filters.City.ToLower()}%' and "
                        + $"LOWER(COALESCE(f.address,'')) like '%{D.Filters.Address.ToLower()}%' and "
                        + (!D.Filters.Archival ? $"f.archival=false and " : "")
                        + (D.Filters.Group > 0 ? $"g.owner={D.Filters.Group} and " : "");

            D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
