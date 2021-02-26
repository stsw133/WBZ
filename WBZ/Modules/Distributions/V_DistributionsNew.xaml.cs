using StswExpress.Globals;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WBZ.Models;
using WBZ.Modules._base;
using WBZ.Modules.Articles;
using WBZ.Modules.Families;
using WBZ.Other;
using MODULE_MODEL = WBZ.Models.M_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsNew.xaml
	/// </summary>
	public partial class DistributionsNew : New
	{
		D_DistributionsNew D = new D_DistributionsNew();

		public DistributionsNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;

			if (mode == Commands.Type.EDIT && D.InstanceInfo.Status != (short)MODULE_MODEL.DistributionStatus.Buffer)
				D.Mode = Commands.Type.PREVIEW;

			D.InstanceInfo.Families = SQL.GetDistributionPositions(D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.DUPLICATE))
				foreach (var family in D.InstanceInfo.Families)
					foreach (DataRow row in family.Positions.Rows)
						row.SetAdded();
		}
		/*
		/// <summary>
		/// Save
		/// </summary>
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			int counter = 0;
			foreach (var family in D.InstanceInfo.Families)
			{
				counter += family.Positions.Rows.Count;
				if (chckToBuffer.IsChecked == false && family.Status != (short)C_DistributionFamily.DistributionFamilyStatus.Taken)
				{
					MessageBox.Show("Dystrybucję można zatwierdzić tylko po rozdaniu całej planowanej żywności!");
					return;
				}
			}

			if (counter == 0)
			{
				MessageBox.Show("Należy dodać co najmniej jedną pozycję do dystrybucji!");
				return;
			}

			D.InstanceInfo.Status = (short)(chckToBuffer.IsChecked == true ? MODULE_MODEL.DistributionStatus.Buffer : MODULE_MODEL.DistributionStatus.Approved);
			if (saved = SQL.SetInstance(D.MODULE_TYPE, D.InstanceInfo, D.Mode))
				Close();
		}
		*/
		/// <summary>
		/// Print
		/// </summary>
		private void btnPrint_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}
		private void btnDistributionList_Click(object sender, RoutedEventArgs e)
		{
			Prints.Print_DistributionList(D.InstanceInfo);
		}

		/// <summary>
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			M_DistributionFamily family;

			var window = new FamiliesList(StswExpress.Globals.Commands.Type.SELECT);
			if (window.ShowDialog() == true)
			{
				family = D.InstanceInfo.Families.FirstOrDefault(x => x.Family == window.Selected.ID);
				if (family == null)
				{
					family = new M_DistributionFamily()
					{
						Family = window.Selected.ID,
						FamilyName = window.Selected.Lastname,
						Members = window.Selected.Members
					};
					D.InstanceInfo.Families.Add(family);
					((CollectionViewSource)gridGroups.Resources["groups"]).View.Refresh();
				}
			}
			else
				return;

			bool br = false;
			do
			{
				var window2 = new ArticlesList(StswExpress.Globals.Commands.Type.SELECT);
				if (window2.ShowDialog() == true)
				{
					if (window2.Selected != null)
					{
						var row = family.Positions.NewRow();

						int counter = 0;
						foreach (var f in D.InstanceInfo.Families)
							counter += f.Positions.Rows.Count;

						row["position"] = counter + 1;
						row["store"] = (window2.DataContext as D_ArticlesList).Filters.MainStore.ID;
						row["storename"] = (window2.DataContext as D_ArticlesList).Filters.MainStore.Name;
						row["article"] = window2.Selected.ID;
						row["articlename"] = window2.Selected.Name;
						row["amount"] = DBNull.Value;
						row["measure"] = window2.Selected.Measure;

						family.Positions.Rows.Add(row);
					}
				}
				else
					br = true;
			} while (!br);
		}

		/// <summary>
		/// ChangeFamilyStatus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnChangeFamilyStatus_Click(object sender, RoutedEventArgs e)
		{
			var family = (((sender as Button).Parent as StackPanel).Tag) as M_DistributionFamily;

			var window = new DistributionsStatus(family);
			window.Owner = this;
			if (window.ShowDialog() == true)
			{
				if (window.rbStatus0.IsChecked == true) family.Status = 0;
				if (window.rbStatus1.IsChecked == true) family.Status = 1;
				if (window.rbStatus2.IsChecked == true) family.Status = 2;
				
				foreach (DataRow pos in family.Positions.Rows)
					if (pos.RowState == DataRowState.Unchanged)
						pos.SetModified();
				((CollectionViewSource)gridGroups.Resources["groups"]).View.Refresh();
			}
		}
	}

	public class New : ModuleNew<MODULE_MODEL> { }
}
