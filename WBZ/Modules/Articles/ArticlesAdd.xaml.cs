using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Stores;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesAdd.xaml
	/// </summary>
	public partial class ArticlesAdd : Window
	{
		M_ArticlesAdd M = new M_ArticlesAdd();

		public ArticlesAdd(C_Article instance, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.InstanceInfo.Measures = SQL.GetArticleMeasures(M.InstanceInfo.ID);
			M.EditMode = editMode;
		}

		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		#region buttons
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (SQL.SetArticle(M.InstanceInfo))
				Close();
		}
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (M.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		private void dgMeasures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			int index = e.Row.GetIndex();

			dgMeasures_Convert(index);
		}

		private async void dgMeasures_Convert(int index)
		{
			await Task.Delay(10);
			await Task.Run(() => {
				if (M.InstanceInfo.ID != 0 && index < M.InstanceInfo.Measures.Rows.Count)
				{
					double conv = Convert.IsDBNull(M.InstanceInfo.Measures.Rows[index]["converter"]) ? 1 : (double)M.InstanceInfo.Measures.Rows[index]["converter"];
					M.InstanceInfo.Measures.Rows[index]["amount"] = Convert.ToDouble(M.InstanceInfo.AmountRaw) / conv;
					M.InstanceInfo.Measures.Rows[index]["reserved"] = Convert.ToDouble(M.InstanceInfo.ReservedRaw) / conv;
				}
			});
		}

		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Stores")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Stores == null)
					M.InstanceSources_Stores = SQL.ListStores($"sa.article={M.InstanceInfo.ID}");
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Documents == null)
					M.InstanceSources_Documents = SQL.ListDocuments($"dp.article={M.InstanceInfo.ID}");
			}
			else if (tab?.Name == "tabSources_Distributions")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Distributions == null)
					M.InstanceSources_Distributions = SQL.ListDistributions($"dp.article={M.InstanceInfo.ID}");
			}
		}

		private void dgList_Stores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.SAVE}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Store>().Select(x => M.InstanceSources_Stores.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new StoresAdd(M.InstanceSources_Stores[index], true);
						window.Show();
					}
				}
				else if (Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.PREVIEW}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Store>().Select(x => M.InstanceSources_Stores.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new StoresAdd(M.InstanceSources_Stores[index], false);
						window.Show();
					}
				}
			}
		}

		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Document>().Select(x => M.InstanceSources_Documents.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new DocumentsAdd(M.InstanceSources_Documents[index], true);
						window.Show();
					}
				}
				else if (Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.PREVIEW}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Document>().Select(x => M.InstanceSources_Documents.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new DocumentsAdd(M.InstanceSources_Documents[index], false);
						window.Show();
					}
				}
			}
		}

		private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Distribution>().Select(x => M.InstanceSources_Distributions.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new DistributionsAdd(M.InstanceSources_Distributions[index], true);
						window.Show();
					}
				}
				else if (Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.PREVIEW}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Distribution>().Select(x => M.InstanceSources_Distributions.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new DistributionsAdd(M.InstanceSources_Distributions[index], false);
						window.Show();
					}
				}
			}
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_ArticlesAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.Module.ARTICLES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private C_Article instanceInfo;
		public C_Article InstanceInfo
		{
			get
			{
				return instanceInfo;
			}
			set
			{
				instanceInfo = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Źródło instancji - magazyny
		private List<C_Store> instanceSources_Stores;
		public List<C_Store> InstanceSources_Stores
		{
			get
			{
				return instanceSources_Stores;
			}
			set
			{
				instanceSources_Stores = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Źródło instancji - dokumenty
		private List<C_Document> instanceSources_Documents;
		public List<C_Document> InstanceSources_Documents
		{
			get
			{
				return instanceSources_Documents;
			}
			set
			{
				instanceSources_Documents = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Źródło instancji - dystrybucje
		private List<C_Distribution> instanceSources_Distributions;
		public List<C_Distribution> InstanceSources_Distributions
		{
			get
			{
				return instanceSources_Distributions;
			}
			set
			{
				instanceSources_Distributions = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Czy okno jest w trybie edycji (zamiast w trybie dodawania)
		public bool IsEditing { get { return InstanceInfo.ID > 0; } }
		/// Tryb edycji dla okna
		public bool EditMode { get; set; }
		/// Ikona okna
		public string EditIcon
		{
			get
			{
				if (InstanceInfo.ID == 0)
					return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
				else if (EditMode)
					return "pack://siteoforigin:,,,/Resources/icon32_edit.ico";
				else
					return "pack://siteoforigin:,,,/Resources/icon32_search.ico";
			}
		}
		/// Tytuł okna
		public string Title
		{
			get
			{
				if (InstanceInfo.ID == 0)
					return "Nowy towar";
				else if (EditMode)
					return $"Edycja towaru: {InstanceInfo.Name}";
				else
					return $"Podgląd towaru: {InstanceInfo.Name}";
			}
		}

		/// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
