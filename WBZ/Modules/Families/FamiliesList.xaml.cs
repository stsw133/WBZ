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

		/// Selected
        public MODULE_MODEL Selected;
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
