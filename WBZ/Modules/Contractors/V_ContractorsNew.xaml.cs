using StswExpress;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Contractor;

namespace WBZ.Modules.Contractors
{
    /// <summary>
    /// Logika interakcji dla klasy ContractorsNew.xaml
    /// </summary>
    public partial class ContractorsNew : New
    {
        readonly D_ContractorsNew D = new D_ContractorsNew();

        public ContractorsNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceData = instance;
            D.Mode = mode;
        }

		/// <summary>
		/// Tab changed for source
		/// </summary>
		private void tcSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name?.EndsWith("_Documents") == true)
            {
				if (D.InstanceData.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.GetModule(nameof(Documents)), $"d.contractor={D.InstanceData.ID}");
            }
        }
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e) => DtgSourceList_MouseDoubleClick<M_Document>(sender, e, Config.GetModule(nameof(Modules.Documents)));

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.City))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano miasta!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Address))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano adresu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Postcode))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano kodu pocztowego!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

    public class New : ModuleNew<MODULE_MODEL> { }
}
