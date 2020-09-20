using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Stores;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsAdd.xaml
	/// </summary>
	public partial class DocumentsAdd : Window
	{
		M_DocumentsAdd M = new M_DocumentsAdd();

		public DocumentsAdd(C_Document instance, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			chckToBuffer.IsChecked = instance.Status == (short)C_Document.DocumentStatus.Buffer;
			M.InstanceInfo.Positions = SQL.GetDocumentPositions(M.InstanceInfo.ID);
			M.EditMode = editMode && M.InstanceInfo.Status == (short)C_Document.DocumentStatus.Buffer;
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

			if (M.InstanceInfo.Positions.Rows.Count == 0)
			{
				MessageBox.Show("Należy dodać co najmniej jedną pozycję do faktury!");
				return;
			}

			M.InstanceInfo.Status = (short)(chckToBuffer.IsChecked==true ? C_Document.DocumentStatus.Buffer : C_Document.DocumentStatus.Approved);
			if (SQL.SetDocument(M.InstanceInfo))
				Close();
		}
		private void btnGetFromFile_Click(object sender, MouseButtonEventArgs e)
		{
			
		}
		private void btnGetByScaner_Click(object sender, MouseButtonEventArgs e)
		{
			
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

		private void btnCompanyChoose_Click(object sender, RoutedEventArgs e)
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

		private void btnStoreChoose_Click(object sender, RoutedEventArgs e)
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
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_DocumentsAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.ModuleTypes.DOCUMENTS;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private C_Document instanceInfo;
		public C_Document InstanceInfo
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
					return "Nowy dokument";
				else if (EditMode)
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
