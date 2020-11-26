using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Interfaces;
using WBZ.Modules.Articles;
using WBZ.Modules.Companies;
using WBZ.Modules.Stores;
using MODULE_MODEL = WBZ.Models.M_Document;

namespace WBZ.Modules.Documents
{
	/// <summary>
	/// Interaction logic for DocumentsNew.xaml
	/// </summary>
	public partial class DocumentsNew : New
	{
		D_DocumentsNew D = new D_DocumentsNew();

		public DocumentsNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;

			if (mode == Commands.Type.EDIT && instance.Status == (short)MODULE_MODEL.DocumentStatus.Buffer)
				D.Mode = Commands.Type.PREVIEW;

			chckToBuffer.IsChecked = instance.Status == (short)MODULE_MODEL.DocumentStatus.Buffer;
			D.InstanceInfo.Positions = SQL.GetInstancePositions(D.MODULE_TYPE, D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.DUPLICATE))
				foreach (DataRow row in D.InstanceInfo.Positions.Rows)
					row.SetAdded();
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
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
