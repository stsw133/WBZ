using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Controls;
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
        D_ContractorsNew D = new D_ContractorsNew();

        public ContractorsNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;
        }

        /// <summary>
		/// Tab changed
		/// </summary>
        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name == "tabSources_Documents")
            {
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.Modules.DOCUMENTS, $"c.id={D.InstanceInfo.ID}");
            }
        }

        /// <summary>
		/// Open: Document
		/// </summary>
        private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dgList_Module_MouseDoubleClick<M_Document>(sender, e, Config.Modules.DOCUMENTS);
        }

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceInfo.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceInfo.City))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano miasta!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceInfo.Address))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano adresu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceInfo.Postcode))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano kodu pocztowego!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

    public class New : ModuleNew<MODULE_MODEL> { }
}
