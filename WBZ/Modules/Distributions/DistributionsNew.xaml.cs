using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Families;
using WBZ.Other;
using MODULE_CLASS = WBZ.Models.C_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsNew.xaml
	/// </summary>
	public partial class DistributionsNew : Window
	{
		D_DistributionsNew D = new D_DistributionsNew();

		public DistributionsNew(MODULE_CLASS instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;

			D.InstanceInfo = instance;
			D.Mode = mode;
			if (mode == Commands.Type.EDIT && instance.Status == (short)MODULE_CLASS.DistributionStatus.Buffer)
				D.Mode = Commands.Type.PREVIEW;

			chckToBuffer.IsChecked = instance.Status == (short)MODULE_CLASS.DistributionStatus.Buffer;
			D.InstanceInfo.Families = SQL.GetDistributionPositions(D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
			{
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
				foreach (var family in D.InstanceInfo.Families)
					foreach (DataRow row in family.Positions.Rows)
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

			D.InstanceInfo.Status = (short)(chckToBuffer.IsChecked == true ? MODULE_CLASS.DistributionStatus.Buffer : MODULE_CLASS.DistributionStatus.Approved);
			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Print
		/// </summary>
		private void btnPrint_Click(object sender, MouseButtonEventArgs e)
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
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			C_DistributionFamily family;

			var window = new FamiliesList(true);
			if (window.ShowDialog() == true)
			{
				family = D.InstanceInfo.Families.FirstOrDefault(x => x.Family == window.Selected.ID);
				if (family == null)
				{
					family = new C_DistributionFamily()
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
				var window2 = new ArticlesList(true);
				if (window2.ShowDialog() == true)
				{
					if (window2.Selected != null)
					{
						var row = family.Positions.NewRow();

						int counter = 0;
						foreach (var f in D.InstanceInfo.Families)
							counter += f.Positions.Rows.Count;

						row["position"] = counter + 1;
						row["store"] = window2.SelectedStore.ID;
						row["storename"] = window2.SelectedStore.Name;
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

		private void btnChangeFamilyStatus_Click(object sender, RoutedEventArgs e)
		{
			var family = (((sender as Button).Parent as StackPanel).Tag) as C_DistributionFamily;

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

		private void Window_Closed(object sender, System.EventArgs e)
		{
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
