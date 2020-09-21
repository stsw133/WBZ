using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Documents;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresAdd.xaml
	/// </summary>
	public partial class StoresAdd : Window
	{
		M_StoresAdd M = new M_StoresAdd();

		public StoresAdd(C_Store instance, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
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

			if (SQL.SetStore(M.InstanceInfo))
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

		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Articles")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Articles == null)
					M.InstanceSources_Articles = SQL.ListArticles($"sa.store={M.InstanceInfo.ID}");
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Documents == null)
					M.InstanceSources_Documents = SQL.ListDocuments($"d.store={M.InstanceInfo.ID}");
			}
		}

		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.SAVE}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Article>().Select(x => M.InstanceSources_Articles.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new ArticlesAdd(M.InstanceSources_Articles[index], true);
						window.Show();
					}
				}
				else if (Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.PREVIEW}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Article>().Select(x => M.InstanceSources_Articles.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new ArticlesAdd(M.InstanceSources_Articles[index], false);
						window.Show();
					}
				}
			}
		}

		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.SAVE}"))
				{
					var indexes = (sender as DataGrid).SelectedItems.Cast<C_Document>().Select(x => M.InstanceSources_Documents.IndexOf(x));
					foreach (int index in indexes)
					{
						var window = new DocumentsAdd(M.InstanceSources_Documents[index], true);
						window.Show();
					}
				}
				else if (Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.PREVIEW}"))
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
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_StoresAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.ModuleTypes.STORES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private C_Store instanceInfo;
		public C_Store InstanceInfo
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
		/// Źródło instancji - towary
		private List<C_Article> instanceSources_Articles;
		public List<C_Article> InstanceSources_Articles
		{
			get
			{
				return instanceSources_Articles;
			}
			set
			{
				instanceSources_Articles = value;
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
		/// Atrybuty
		private List<C_Attribute> instanceAttributes;
		public List<C_Attribute> InstanceAttributes
		{
			get
			{
				return instanceAttributes;
			}
			set
			{
				instanceAttributes = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Załączniki
		private List<C_Attachment> instanceAttachments;
		public List<C_Attachment> InstanceAttachments
		{
			get
			{
				return instanceAttachments;
			}
			set
			{
				instanceAttachments = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Logi
		private List<C_Log> instanceLogs;
		public List<C_Log> InstanceLogs
		{
			get
			{
				return instanceLogs;
			}
			set
			{
				instanceLogs = value;
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
					return "Nowy magazyn";
				else if (EditMode)
					return $"Edycja magazynu: {InstanceInfo.Name}";
				else
					return $"Podgląd magazynu: {InstanceInfo.Name}";
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
