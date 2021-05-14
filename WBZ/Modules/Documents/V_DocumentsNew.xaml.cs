﻿using StswExpress;
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
				D.InstanceData = instance;
			D.Mode = mode;

			if (mode == Commands.Type.EDIT && D.InstanceData.Status != (short)MODULE_MODEL.DocumentStatus.Buffer)
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
		/// Select: Contractor
		/// </summary>
		private void btnSelectContractor_Click(object sender, RoutedEventArgs e)
		{
			var window = new ContractorsList(Commands.Type.SELECT);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					D.InstanceData.Contractor = window.Selected.ID;
					D.InstanceData = D.InstanceData;
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
					D.InstanceData.Store = window.Selected.ID;
					D.InstanceData = D.InstanceData;
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
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano typu dokumentu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (string.IsNullOrEmpty(D.InstanceData.Name))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie podano nazwy (numeru) dokumentu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.cStore == null)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano magazynu!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.Contractor == 0)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano kontrahenta!") { Owner = this }.ShowDialog();
				return false;
			}
			if (D.InstanceData.Positions.Rows.Count == 0)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Brak pozycji na dokumencie!") { Owner = this }.ShowDialog();
				return false;
			}

			return true;
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
