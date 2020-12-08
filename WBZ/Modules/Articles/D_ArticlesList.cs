using System.Collections.Generic;
using System.Collections.Specialized;
using WBZ.Globals;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    class D_ArticlesList : D_ModuleList<MODULE_MODEL>
	{
		/// Module
		public readonly string MODULE_TYPE = Global.Module.ARTICLES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_ArticlesList;
		
		/// Stores list
		public List<M_ComboValue> StoresList { get; } = SQL.ComboInstances(Global.Module.STORES, "codename", "archival=false");
	}
}
