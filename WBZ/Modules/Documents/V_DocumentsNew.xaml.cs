using StswExpress.Globals;
using System;
using System.Data;
using System.Windows;
using WBZ.Modules._base;
using WBZ.Modules.Articles;
using WBZ.Modules.Contractors;
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

			if (mode == Commands.Type.EDIT && D.InstanceInfo.Status != (short)MODULE_MODEL.DocumentStatus.Buffer)
				D.Mode = Commands.Type.PREVIEW;

			D.InstanceInfo.Positions = SQL.GetInstancePositions(D.MODULE_TYPE, D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.DUPLICATE))
				foreach (DataRow row in D.InstanceInfo.Positions.Rows)
					row.SetAdded();
		}

		/// <summary>
		/// GetFromFile
		/// </summary>
		private void btnGetFromFile_Click(object sender, RoutedEventArgs e)
		{
			
		}

		/// <summary>
		/// GetByScaner
		/// </summary>
		private void btnGetByScaner_Click(object sender, RoutedEventArgs e)
		{
			
		}

		/// <summary>
		/// Select: Contractor
		/// </summary>
		private void btnSelectContractor_Click(object sender, RoutedEventArgs e)
		{
			var window = new ContractorsList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceInfo.Contractor = window.Selected.ID;
					D.InstanceInfo.ContractorName = window.Selected.Name;
					D.InstanceInfo = D.InstanceInfo;
				}
		}

		/// <summary>
		/// Select: Store
		/// </summary>
		private void btnSelectStore_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList(Commands.Type.SELECT);
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
			var window = new ArticlesList(Commands.Type.SELECT);
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
