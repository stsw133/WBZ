using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.C_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresNew.xaml
	/// </summary>
	public partial class StoresNew : New
	{
		D_StoresNew D = new D_StoresNew();

		public StoresNew(MODULE_MODEL instance, Commands.Type mode)
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
			if (tab?.Name == "tabSources_Articles")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Articles == null)
					D.InstanceSources_Articles = SQL.ListInstances<C_Article>(Global.Module.ARTICLES, $"sa.store={D.InstanceInfo.ID}");
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Documents == null)
					D.InstanceSources_Documents = SQL.ListInstances<C_Document>(Global.Module.DOCUMENTS, $"d.store={D.InstanceInfo.ID}");
			}
		}

		/// <summary>
		/// Open: Article
		/// </summary>
		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			dgList_Module_MouseDoubleClick<C_Article>(sender, e, Global.Module.ARTICLES);
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			dgList_Module_MouseDoubleClick<C_Document>(sender, e, Global.Module.DOCUMENTS);
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
