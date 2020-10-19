using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Stores;
using MODULE_CLASS = WBZ.Classes.C_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsNew.xaml
	/// </summary>
	public partial class DocumentsNew : Window
	{
		M_DocumentsNew M = new M_DocumentsNew();

		public DocumentsNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.Mode = mode;
			if (mode == Global.ActionType.EDIT && instance.Status == (short)MODULE_CLASS.DocumentStatus.Buffer)
				M.Mode = Global.ActionType.PREVIEW;

			chckToBuffer.IsChecked = instance.Status == (short)MODULE_CLASS.DocumentStatus.Buffer;
			M.InstanceInfo.Positions = SQL.GetInstancePositions(M.MODULE_NAME, M.InstanceInfo.ID);
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
			{
				M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
				foreach (DataRow row in M.InstanceInfo.Positions.Rows)
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

			if (M.InstanceInfo.Positions.Rows.Count == 0)
			{
				MessageBox.Show("Należy dodać co najmniej jedną pozycję do faktury!");
				return;
			}

			M.InstanceInfo.Status = (short)(chckToBuffer.IsChecked==true ? MODULE_CLASS.DocumentStatus.Buffer : MODULE_CLASS.DocumentStatus.Approved);
			if (saved = SQL.SetInstance(M.MODULE_NAME, M.InstanceInfo, M.Mode))
				Close();
		}

		/// <summary>
		/// GetFromFile
		/// </summary>
		private void btnGetFromFile_Click(object sender, MouseButtonEventArgs e)
		{
			
		}

		/// <summary>
		/// GetByScaner
		/// </summary>
		private void btnGetByScaner_Click(object sender, MouseButtonEventArgs e)
		{
			
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
		/// Select: Company
		/// </summary>
		private void btnSelectCompany_Click(object sender, RoutedEventArgs e)
		{
			var window = new CompaniesList(true);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					M.InstanceInfo.Company = window.Selected.ID;
					M.InstanceInfo.CompanyName = window.Selected.Name;
					M.InstanceInfo = M.InstanceInfo;
				}
		}

		/// <summary>
		/// Select: Store
		/// </summary>
		private void btnSelectStore_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList(true);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					M.InstanceInfo.Store = window.Selected.ID;
					M.InstanceInfo.StoreName = window.Selected.Name;
					M.InstanceInfo = M.InstanceInfo;
				}
		}

		/// <summary>
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList(true);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					var row = M.InstanceInfo.Positions.NewRow();

					row["position"] = M.InstanceInfo.Positions.Rows.Count + 1;
					row["article"] = window.Selected.ID;
					row["articlename"] = window.Selected.Name;
					row["amount"] = DBNull.Value;
					row["measure"] = window.Selected.Measure;
					row["cost"] = DBNull.Value;

					M.InstanceInfo.Positions.Rows.Add(row);
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
	internal class M_DocumentsNew : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.DOCUMENTS;

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
					return "Nowy dokument";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie dokumentu: {InstanceInfo.Name}";
				else if (Mode == Global.ActionType.EDIT)
					return $"Edycja dokumentu: {InstanceInfo.Name}";
				else
					return $"Podgląd dokumentu: {InstanceInfo.Name}";
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
