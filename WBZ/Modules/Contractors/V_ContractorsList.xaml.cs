using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    /// <summary>
    /// Logika interakcji dla klasy ContractorsList.xaml
    /// </summary>
    public partial class ContractorsList : List
    {
        readonly D_ContractorsList D = new D_ContractorsList();

        public ContractorsList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            D.Mode = mode;
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
