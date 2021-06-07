using StswExpress;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesList.xaml
	/// </summary>
	public partial class AttributesClassesList : List
	{
        readonly D_IconsList D = new D_IconsList();

		public AttributesClassesList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
