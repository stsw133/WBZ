using System.Data;
using WBZ.Controls;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesNew.xaml
	/// </summary>
	public partial class AttributesClassesNew : New
	{
        D_IconsNew D = new D_IconsNew();

        public AttributesClassesNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;

            D.InstanceInfo.Values = SQL.GetInstancePositions(D.MODULE_TYPE, D.InstanceInfo.ID);
            if (D.Mode == StswExpress.Globals.Commands.Type.DUPLICATE)
                foreach (DataRow row in D.InstanceInfo.Values.Rows)
                    row.SetAdded();
        }

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceInfo.Module))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano modułu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceInfo.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceInfo.Type))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano typu!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

    public class New : ModuleNew<MODULE_MODEL> { }
}
