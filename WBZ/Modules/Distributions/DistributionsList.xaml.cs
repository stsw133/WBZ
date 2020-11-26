using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsList.xaml
	/// </summary>
	public partial class DistributionsList : List
	{
		D_DistributionsList D = new D_DistributionsList();

		public DistributionsList(Commands.Type mode)
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
