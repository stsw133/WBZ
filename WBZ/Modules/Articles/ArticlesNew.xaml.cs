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
using MODULE_CLASS = WBZ.Classes.C_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesNew.xaml
	/// </summary>
	public partial class ArticlesNew : Window
	{
		M_ArticlesNew M = new M_ArticlesNew();

		public ArticlesNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.Mode = mode;

			M.InstanceInfo.Measures = SQL.GetArticleMeasures(M.InstanceInfo.ID);
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
			{
				M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
				foreach (DataRow row in M.InstanceInfo.Measures.Rows)
					row.SetAdded();
			}
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

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Stores")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Stores == null)
					M.InstanceSources_Stores = SQL.ListInstances(Global.Module.STORES, $"sa.article={M.InstanceInfo.ID}").DataTableToList<C_Store>();
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Documents == null)
					M.InstanceSources_Documents = SQL.ListInstances(Global.Module.DOCUMENTS, $"dp.article={M.InstanceInfo.ID}").DataTableToList<C_Document>();
			}
			else if (tab?.Name == "tabSources_Distributions")
			{
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Distributions == null)
					M.InstanceSources_Distributions = SQL.ListInstances(Global.Module.DISTRIBUTIONS, $"dp.article={M.InstanceInfo.ID}").DataTableToList<C_Distribution>();
			}
		}

		/// <summary>
		/// Open: Store
		/// </summary>
		private void dgList_Stores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Store>();
				foreach (C_Store instance in selectedInstances)
				{
					var window = new StoresNew(instance, perm);
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

		/// <summary>
		/// Open: Distribution
		/// </summary>
		private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Global.ActionType perm = Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}")
					? Global.ActionType.EDIT : Global.ActionType.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Distribution>();
				foreach (C_Distribution instance in selectedInstances)
				{
					var window = new DistributionsNew(instance, perm);
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
	internal class M_ArticlesNew : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.ARTICLES;

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
		/// Instance source - stores
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
		/// Instance source - distributions
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
		/// Editing mode
		public bool EditingMode { get { return Mode != Global.ActionType.PREVIEW; } }
		/// Window mode
		public Global.ActionType Mode { get; set; }
		/// Additional window icon
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
		/// Window title
		public string Title
		{
			get
			{
				if (Mode == Global.ActionType.NEW)
					return "Nowy towar";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie towaru: {InstanceInfo.Name}";
				else if (Mode == Global.ActionType.EDIT)
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
