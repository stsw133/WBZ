using StswExpress;
using System.Data;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesNew.xaml
	/// </summary>
	public partial class AttributesClassesNew : New
	{
        readonly D_AttributesClassesNew D = new D_AttributesClassesNew();

        public AttributesClassesNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceData = instance;
            D.Mode = mode;

            D.InstanceData.Values = SQL.GetInstancePositions(D.Module.Value.ToString(), D.InstanceData.ID);
            if (D.Mode == Commands.Type.DUPLICATE)
                foreach (DataRow row in D.InstanceData.Values.Rows)
                    row.SetAdded();
        }

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceData.Module))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano modułu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Type))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano typu!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

    public class New : ModuleNew<MODULE_MODEL> { }
}
