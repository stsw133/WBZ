using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    class D_ArticlesList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.ARTICLES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_ArticlesList;
		
		/// Stores list
		public ObservableCollection<M_ComboValue> StoresList { get; } = SQL.ComboInstances(Global.Module.STORES, "codename", "archival=false", true);
	}
}
