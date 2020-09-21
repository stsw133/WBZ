using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Admin;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Families;
using WBZ.Modules.Stores;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for LogsList.xaml
	/// </summary>
	public partial class LogsList : Window
	{
		M_LogsList M = new M_LogsList();

		public LogsList()
		{
			InitializeComponent();
			DataContext = M;
			btnRefresh_Click(null, null);

			if (C_Config.Logs_Enabled == "1")
				chckEnabled.IsChecked = true;
			else
				chckEnabled.IsChecked = false;
		}

		/// <summary>
		/// Aktualizacja filtrów wyszukiwania
		/// </summary>
		#region filters
		private void UpdateFilters()
		{
			M.FilterSQL = $"LOWER(COALESCE(u.lastname,'') || ' ' || COALESCE(u.forename,'')) like '%{M.Filters.UserFullname.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.module,'')) like '%{M.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(l.content,'')) like '%{M.Filters.Content.ToLower()}%' and "
						+ $"l.datetime between '{M.Filters.fDateTime:yyyy-MM-dd}' and '{M.Filters.DateTime:yyyy-MM-dd} 23:59:59' and ";

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new C_Log();
			btnRefresh_Click(null, null);
		}
		#endregion

		#region buttons
		private void btnPreview_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Log>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var log = M.InstancesList[index];
				openObj(log, false);
			}
		}
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Log>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var log = M.InstancesList[index];
				openObj(log, true);
			}
		}
		private void openObj(C_Log log, bool editMode)
		{
			if (log.Instance == 0)
				return;

			if (log.Module == Global.ModuleTypes.DOCUMENTS)
			{
				if (SQL.CountInstances(log.Module, $"d.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.DOCUMENTS}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new DocumentsAdd(SQL.GetInstance(log.Module, log.Instance) as C_Document, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.STORES)
			{
				if (SQL.CountInstances(log.Module, $"s.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.STORES}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.STORES}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.STORES}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new StoresAdd(SQL.GetInstance(log.Module, log.Instance) as C_Store, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.ARTICLES)
			{
				if (SQL.CountInstances(log.Module, $"a.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.ARTICLES}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new ArticlesAdd(SQL.GetInstance(log.Module, log.Instance) as C_Article, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.COMPANIES)
			{
				if (SQL.CountInstances(log.Module, $"c.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.COMPANIES}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.COMPANIES}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.COMPANIES}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new CompaniesAdd(SQL.GetInstance(log.Module, log.Instance) as C_Company, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.FAMILIES)
			{
				if (SQL.CountInstances(log.Module, $"f.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.FAMILIES}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.FAMILIES}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.FAMILIES}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new FamiliesAdd(SQL.GetInstance(log.Module, log.Instance) as C_Family, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.DISTRIBUTIONS)
			{
				if (SQL.CountInstances(log.Module, $"d.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.DISTRIBUTIONS}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.DISTRIBUTIONS}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.DISTRIBUTIONS}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new DistributionsAdd(SQL.GetInstance(log.Module, log.Instance) as C_Distribution, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.USERS)
			{
				if (SQL.CountInstances(log.Module, $"u.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.USERS}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.USERS}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.USERS}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new UsersAdd(SQL.GetInstance(log.Module, log.Instance) as C_User, editMode);
				window.Show();
			}
			else if (log.Module == Global.ModuleTypes.ATTRIBUTES_CLASSES)
			{
				if (SQL.CountInstances(log.Module, $"ac.id={log.Instance}") == 0)
					return;

				if (!(editMode && Global.User.Perms.Contains($"{Global.ModuleTypes.ATTRIBUTES_CLASSES}_{Global.UserPermTypes.SAVE}")))
					editMode = false;
				if (!Global.User.Perms.Contains($"{Global.ModuleTypes.ATTRIBUTES_CLASSES}_{Global.UserPermTypes.PREVIEW}") && !Global.User.Perms.Contains($"{Global.ModuleTypes.ATTRIBUTES_CLASSES}_{Global.UserPermTypes.SAVE}"))
					return;
				var window = new AttributesClassesAdd(SQL.GetInstance(log.Module, log.Instance) as C_AttributeClass, editMode);
				window.Show();
			}
		}
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			var indexes = dgList.SelectedItems.Cast<C_Log>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteLog(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
		}
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(Global.ModuleTypes.LOGS, M.FilterSQL);
				M.InstancesList = SQL.ListLogs(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "datetime", true);
				foreach (var instance in M.InstancesList)
					instance.TranslatedModule = Global.TranslateModules(instance.Module);
			});
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				btnEdit_Click(null, null);
		}

		private void dgList_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange > 0 && e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && M.InstancesList.Count < M.TotalItems)
			{
				DataContext = null;
				M.InstancesList.AddRange(SQL.ListLogs(M.FilterSQL, M.Limit, ++M.Page * M.Limit, "datetime", true));
				for (int i = M.InstancesList.Count - 1; i >= Math.Max(M.InstancesList.Count - M.Limit, 0); i--)
					M.InstancesList[i].TranslatedModule = Global.TranslateModules(M.InstancesList[i].Module);
				DataContext = M;
				Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToVerticalOffset(e.VerticalOffset);
			}
		}

		private void chckEnabled_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
				SQL.SetPropertyValue("LOGS_ENABLED", "1");
			else
				SQL.SetPropertyValue("LOGS_ENABLED", "0");
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_LogsList : INotifyPropertyChanged
	{
		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Lista instancji
		private List<C_Log> instancesList;
		public List<C_Log> InstancesList
		{
			get
			{
				return instancesList;
			}
			set
			{
				instancesList = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Filtr SQL
		public string FilterSQL { get; set; }
		/// Instancja filtra
		private C_Log filters = new C_Log();
		public C_Log Filters
		{
			get
			{
				return filters;
			}
			set
			{
				filters = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Numer strony
		private int page;
		public int Page
		{
			get
			{
				return page;
			}
			set
			{
				page = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Limit instancji na stronę
		public int Limit { get; } = 50;
		/// Łączna liczba instancji
		private int totalItems;
		public int TotalItems
		{
			get
			{
				return totalItems;
			}
			set
			{
				totalItems = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
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
