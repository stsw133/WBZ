using StswExpress.Globals;
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
		public readonly string MODULE_TYPE = Config.Modules.ARTICLES;
		public StringCollection SORTING = Properties.Settings.Default.sorting_ArticlesList;

		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Commands.Type.SELECT)	return "Wybór towaru";
				else								return "Lista towarów";
			}
		}
		/// Stores list
		private ObservableCollection<M_ComboValue> storesList;
		public ObservableCollection<M_ComboValue> StoresList
        {
			get => storesList;
			set => SetField(ref storesList, value, () => StoresList);
		}
	}
}
