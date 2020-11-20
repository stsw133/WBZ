using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Stores;
using MODULE_CLASS = WBZ.Models.C_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsNew.xaml
	/// </summary>
	public partial class DocumentsNew : Window
	{
		D_DocumentsNew D = new D_DocumentsNew();

		public DocumentsNew(MODULE_CLASS instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;

			D.InstanceInfo = instance;
			D.Mode = mode;
			if (mode == Commands.Type.EDIT && instance.Status == (short)MODULE_CLASS.DocumentStatus.Buffer)
				D.Mode = Commands.Type.PREVIEW;

			chckToBuffer.IsChecked = instance.Status == (short)MODULE_CLASS.DocumentStatus.Buffer;
			D.InstanceInfo.Positions = SQL.GetInstancePositions(D.MODULE_NAME, D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
			{
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
				foreach (DataRow row in D.InstanceInfo.Positions.Rows)
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

			if (D.InstanceInfo.Positions.Rows.Count == 0)
			{
				MessageBox.Show("Należy dodać co najmniej jedną pozycję do faktury!");
				return;
			}

			D.InstanceInfo.Status = (short)(chckToBuffer.IsChecked==true ? MODULE_CLASS.DocumentStatus.Buffer : MODULE_CLASS.DocumentStatus.Approved);
			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
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
			if (D.InstanceInfo.ID == 0)
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
			var window = new CompaniesList(Commands.Type.SELECTING);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.Company = window.Selected.ID;
					D.InstanceInfo.CompanyName = window.Selected.Name;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		/// <summary>
		/// Select: Store
		/// </summary>
		private void btnSelectStore_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList(Commands.Type.SELECTING);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.Store = window.Selected.ID;
					D.InstanceInfo.StoreName = window.Selected.Name;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		/// <summary>
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList(Commands.Type.SELECTING);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					var row = D.InstanceInfo.Positions.NewRow();

					row["position"] = D.InstanceInfo.Positions.Rows.Count + 1;
					row["article"] = window.Selected.ID;
					row["articlename"] = window.Selected.Name;
					row["amount"] = DBNull.Value;
					row["measure"] = window.Selected.Measure;
					row["cost"] = DBNull.Value;

					D.InstanceInfo.Positions.Rows.Add(row);
				}
		}

		/// <summary>
		/// Closed
		/// </summary>
		private void Window_Closed(object sender, EventArgs e)
		{
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
