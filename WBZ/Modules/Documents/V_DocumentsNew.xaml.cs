using StswExpress;
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
		readonly D_DocumentsNew D = new D_DocumentsNew();

		public DocumentsNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceData = instance;
			D.Mode = mode;

			if (mode == Commands.Type.EDIT && D.InstanceData.Status != 0)
				D.Mode = Commands.Type.PREVIEW;

			D.InstanceData.Positions = SQL.GetInstancePositions(D.Module, D.InstanceData.ID);
			if (D.Mode.In(Commands.Type.DUPLICATE))
				foreach (DataRow row in D.InstanceData.Positions.Rows)
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
		/// Select
		/// </summary>
		private void btnSelectContractor_Click(object sender, RoutedEventArgs e)
		{
			var window = new ContractorsList(Commands.Type.SELECT);
			if (window.ShowDialog() == true && window.Selected != null)
			{
				D.InstanceData.ContractorID = window.Selected.ID;
				D.InstanceData = D.InstanceData;
			}
		}
		private void btnSelectStore_Click(object sender, RoutedEventArgs e)
		{
			var window = new StoresList(Commands.Type.SELECT);
			if (window.ShowDialog() == true && window.Selected != null)
			{
				D.InstanceData.StoreID = window.Selected.ID;
				D.InstanceData = D.InstanceData;
			}
		}

		/// <summary>
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new ArticlesList(Commands.Type.SELECT);
			if (window.ShowDialog() == true && window.Selected != null)
			{
				var row = D.InstanceData.Positions.NewRow();
				row["position"] = D.InstanceData.Positions.Rows.Count + 1;
				row["article"] = window.Selected.ID;
				row["articlename"] = window.Selected.Name;
				row["amount"] = DBNull.Value;
				row["measure"] = window.Selected.Measure;
				row["cost"] = DBNull.Value;
				D.InstanceData.Positions.Rows.Add(row);
			}
		}

		/// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
		{
			if (string.IsNullOrEmpty(D.InstanceData.Type))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie wybrano typu dokumentu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano nazwy (numeru) dokumentu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.StoreID == 0)
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie wybrano magazynu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.ContractorID == 0)
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie wybrano kontrahenta!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.Positions.Rows.Count == 0)
			{
				new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Brak pozycji na dokumencie!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
