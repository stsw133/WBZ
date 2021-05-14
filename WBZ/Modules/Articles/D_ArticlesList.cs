﻿using StswExpress;
using System.Collections.ObjectModel;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    class D_ArticlesList : D_ModuleList<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "Lista towarów";
				else if (Mode == Commands.Type.SELECT)	return "Wybór towaru";
				else									return string.Empty;
			}
		}
		/// Stores list
		private ObservableCollection<MV> storesList;
		public ObservableCollection<MV> StoresList
        {
			get => storesList;
			set => SetField(ref storesList, value, () => StoresList);
		}
    }
}
