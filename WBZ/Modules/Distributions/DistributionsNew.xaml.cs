using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Families;
using WBZ.Other;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;
using MODULE_CLASS = WBZ.Classes.C_Distribution;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsNew.xaml
	/// </summary>
	public partial class DistributionsNew : Window
	{
		M_DistributionsNew M = new M_DistributionsNew();

		public DistributionsNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.Mode = mode;
			if (mode == Global.ActionType.EDIT && instance.Status == (short)MODULE_CLASS.DistributionStatus.Buffer)
				M.Mode = Global.ActionType.PREVIEW;

			chckToBuffer.IsChecked = instance.Status == (short)MODULE_CLASS.DistributionStatus.Buffer;
			M.InstanceInfo.Families = SQL.GetDistributionPositions(M.InstanceInfo.ID);
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
			{
				M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
				foreach (var family in M.InstanceInfo.Families)
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
			foreach (var family in M.InstanceInfo.Families)
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

			M.InstanceInfo.Status = (short)(chckToBuffer.IsChecked == true ? MODULE_CLASS.DistributionStatus.Buffer : MODULE_CLASS.DistributionStatus.Approved);
			if (saved = SQL.SetInstance(M.MODULE_NAME, M.InstanceInfo, M.Mode))
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
			Prints.Print_DistributionList(M.InstanceInfo);
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
		/// Add position
		/// </summary>
		private void btnPositionsAdd_Click(object sender, RoutedEventArgs e)
		{
			C_DistributionFamily family;

			var window = new FamiliesList(true);
			if (window.ShowDialog() == true)
			{
				family = M.InstanceInfo.Families.FirstOrDefault(x => x.Family == window.Selected.ID);
				if (family == null)
				{
					family = new C_DistributionFamily()
					{
						Family = window.Selected.ID,
						FamilyName = window.Selected.Lastname,
						Members = window.Selected.Members
					};
					M.InstanceInfo.Families.Add(family);
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
						foreach (var f in M.InstanceInfo.Families)
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
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
				SQL.ClearObject(M.MODULE_NAME, M.InstanceInfo.ID);
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_DistributionsNew : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.DISTRIBUTIONS;

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
					return "Nowa dystrybucja";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie dystrybucji: {InstanceInfo.Name}";
				else if (Mode == Global.ActionType.EDIT)
					return $"Edycja dystrybucji: {InstanceInfo.Name}";
				else
					return $"Podgląd dystrybucji: {InstanceInfo.Name}";
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
