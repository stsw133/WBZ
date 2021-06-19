using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsList.xaml
	/// </summary>
	public partial class DistributionsList : List
	{
        readonly D_DistributionsList D = new D_DistributionsList();

		public DistributionsList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
