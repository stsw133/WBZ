using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyList.xaml
    /// </summary>
    public partial class FamiliesList : List
    {
        readonly D_FamiliesList D = new D_FamiliesList();

        public FamiliesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            D.Mode = mode;
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
