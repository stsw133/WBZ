using WBZ.Helpers;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Company;

namespace WBZ.Modules.Companies
{
    /// <summary>
    /// Logika interakcji dla klasy CompaniesList.xaml
    /// </summary>
    public partial class CompaniesList : List
    {
        D_CompaniesList D = new D_CompaniesList();

        public CompaniesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            btnRefresh_Click(null, null);

            D.Mode = mode;
        }

		/// Selected
        public MODULE_MODEL Selected;
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
