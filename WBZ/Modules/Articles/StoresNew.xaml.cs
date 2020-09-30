using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Documents;
using MODULE_CLASS = WBZ.Classes.C_Store;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for StoresNew.xaml
	/// </summary>
	public partial class StoresNew : Window
	{
		M_StoresNew M = new M_StoresNew();

		public StoresNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.Mode = mode;

			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
				M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
		}

		/// <summary>
		/// Validation
		/// </summary>
		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(M.MODULE_NAME, M.InstanceInfo, M.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (M.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Articles")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Articles == null)
					M.InstanceSources_Articles = SQL.ListInstances(Global.Module.ARTICLES, $"sa.store={M.InstanceInfo.ID}").DataTableToList<C_Article>();
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Documents == null)
					M.InstanceSources_Documents = SQL.ListInstances(Global.Module.DOCUMENTS, $"d.store={M.InstanceInfo.ID}").DataTableToList<C_Document>();
			}
		}

		/// <summary>
		/// Open: Article
		/// </summary>
		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.ARTICLES}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Article>();
				foreach (C_Article instance in selectedInstances)
				{
					var window = new ArticlesNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Document>();
				foreach (C_Document instance in selectedInstances)
				{
					var window = new DocumentsNew(instance, perm);
					window.Show();
				}
			}
		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
				SQL.ClearObject(M.MODULE_NAME, M.InstanceInfo.ID);
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_StoresNew : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.STORES;

		/// Logged user
		public C_User User { get; } = Global.User;
		/// Instance
		private MODULE_CLASS instanceInfo;
		public MODULE_CLASS InstanceInfo
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
		/// Instance source - articles
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
		/// Instance source - documents
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
		/// Editing mode
		public bool EditingMode { get { return Mode != Global.ActionType.PREVIEW; } }
		/// Tryb okna
		public Global.ActionType Mode { get; set; }
		/// Dodatkowa ikona okna
		public string ModeIcon
		{
			get
			{
				if (Mode == Global.ActionType.NEW)
					return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
				else if (Mode == Global.ActionType.DUPLICATE)
					return "pack://siteoforigin:,,,/Resources/icon32_duplicate.ico";
				else if (Mode == Global.ActionType.EDIT)
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
				if (Mode == Global.ActionType.NEW)
					return "Nowy magazyn";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie magazynu: {InstanceInfo.Name}";
				else if (Mode == Global.ActionType.EDIT)
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
