using WBZ.Helpers;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesList.xaml
	/// </summary>
	public partial class AttributesClassesList : List
	{
		D_AttributesClassesList D = new D_AttributesClassesList();

		public AttributesClassesList(Commands.Type mode)
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
