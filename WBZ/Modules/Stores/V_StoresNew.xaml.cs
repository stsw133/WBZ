using StswExpress;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
		readonly D_StoresNew D = new D_StoresNew();

		public StoresNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceData = instance;
			D.Mode = mode;
		}

		/// <summary>
		/// Tab changed for sources
		/// </summary>
		private async void tcSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			await Task.Run(() =>
			{
				if (tab?.Name == "tabSources_Articles")
				{
					if (D.InstanceData.ID != 0 && D.InstanceSources_Articles == null)
						D.InstanceSources_Articles = SQL.ListInstances<M_Article>(Config.GetModule(nameof(Modules.Articles)), $"sa.store={D.InstanceData.ID}");
				}
				else if (tab?.Name == "tabSources_Documents")
				{
					if (D.InstanceData.ID != 0 && D.InstanceSources_Documents == null)
						D.InstanceSources_Documents = SQL.ListInstances<M_Document>(Config.GetModule(nameof(Modules.Documents)), $"d.store={D.InstanceData.ID}");
				}
			});
		}
		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e) => dgSourceList_MouseDoubleClick<M_Article>(sender, e, Config.GetModule(nameof(Modules.Articles)));
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e) => dgSourceList_MouseDoubleClick<M_Document>(sender, e, Config.GetModule(nameof(Modules.Documents)));

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
