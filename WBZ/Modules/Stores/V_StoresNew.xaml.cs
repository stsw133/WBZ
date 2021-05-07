using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresNew.xaml
	/// </summary>
	public partial class StoresNew : New
	{
		D_TransportNew D = new D_TransportNew();

		public StoresNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceData = instance;
			D.Mode = mode;
		}

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Articles")
			{
				if (D.InstanceData.ID != 0 && D.InstanceSources_Articles == null)
                    D.InstanceSources_Articles = SQL.ListInstances<M_Article>(Config.Modules.ARTICLES, $"sa.store={D.InstanceData.ID}");
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (D.InstanceData.ID != 0 && D.InstanceSources_Documents == null)
                    D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.Modules.DOCUMENTS, $"d.store={D.InstanceData.ID}");
			}
		}

		/// <summary>
		/// Open: Article
		/// </summary>
		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            dgList_Module_MouseDoubleClick<M_Article>(sender, e, Config.Modules.ARTICLES);
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
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.City))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano miasta!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Address))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano adresu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Postcode))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano kodu pocztowego!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
