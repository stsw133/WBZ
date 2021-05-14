﻿using StswExpress;
using System.Collections.ObjectModel;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    class D_ArticlesNew : D_ModuleNew<MODULE_MODEL>
	{
		/// Window title
		public string Title
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return $"Nowy towar";
				else if (Mode == Commands.Type.DUPLICATE)	return $"Duplikowanie towaru: {InstanceData.Name}";
				else if (Mode == Commands.Type.EDIT)		return $"Edycja towaru: {InstanceData.Name}";
				else if (Mode == Commands.Type.PREVIEW)		return $"Podgląd towaru: {InstanceData.Name}";
				else										return string.Empty;
			}
		}
		/// Instance source - stores
		private ObservableCollection<M_Store> instanceSources_Stores;
		public ObservableCollection<M_Store> InstanceSources_Stores
		{
			get => instanceSources_Stores;
			set => SetField(ref instanceSources_Stores, value, () => InstanceSources_Stores);
		}
		/// Instance source - documents
		private ObservableCollection<M_Document> instanceSources_Documents;
		public ObservableCollection<M_Document> InstanceSources_Documents
		{
			get => instanceSources_Documents;
			set => SetField(ref instanceSources_Documents, value, () => InstanceSources_Documents);
		}
		/// Instance source - distributions
		private ObservableCollection<M_Distribution> instanceSources_Distributions;
		public ObservableCollection<M_Distribution> InstanceSources_Distributions
		{
			get => instanceSources_Distributions;
			set => SetField(ref instanceSources_Distributions, value, () => InstanceSources_Distributions);
		}
	}
}
