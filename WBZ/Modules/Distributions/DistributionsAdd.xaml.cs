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
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsAdd.xaml
	/// </summary>
	public partial class DistributionsAdd : Window
	{
		M_DistributionsAdd M = new M_DistributionsAdd();

		public DistributionsAdd(C_Distribution instance, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			chckToBuffer.IsChecked = instance.Status == (short)C_Distribution.DistributionStatus.Buffer;
			M.InstanceInfo.Families = SQL.GetDistributionPositions(M.InstanceInfo.ID);
			M.EditMode = editMode && M.InstanceInfo.Status == (short)C_Distribution.DistributionStatus.Buffer;
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

			M.InstanceInfo.Status = (short)(chckToBuffer.IsChecked == true ? C_Distribution.DistributionStatus.Buffer : C_Distribution.DistributionStatus.Approved);
			if (SQL.SetDistribution(M.InstanceInfo))
				Close();
		}
		private void btnPrint_Click(object sender, MouseButtonEventArgs e)
		{
			var btn = sender as FrameworkElement;
			if (btn != null)
				btn.ContextMenu.IsOpen = true;
		}
		private void btnGiveawayList_Click(object sender, RoutedEventArgs e)
		{
			Color color = Color.FromRgb(0, 0, 0);
			Color lColor = Color.FromRgb(192, 192, 192);

			///Create document.pdf
			Document document = new Document();
			document.Info.Title = $"Realizacja darowizn";
			document.Info.Subject = $"Realizacja darowizn: {M.InstanceInfo.ID} - {M.InstanceInfo.DateReal}";
			document.Info.Author = $"{Global.Database.Name}";

			///Title
			Section section = document.AddSection();
			section.PageSetup.TopMargin = "4cm";

			Image img = section.Headers.Primary.AddImage("Resources/logo.png");
			img.ScaleHeight = 0.200;
			img.ScaleWidth = 0.200;
			img.Top = ShapePosition.Top;
			img.Left = ShapePosition.Right;
			img.RelativeHorizontal = RelativeHorizontal.Page;
			img.RelativeVertical = RelativeVertical.Page;

			///Subject
			Paragraph pSubject = section.Headers.Primary.AddParagraph("Wielkopolski Bank Żywności");
			pSubject.AddLineBreak();
			pSubject.AddText($"{Global.Database.Name}");
			pSubject.AddLineBreak();
			pSubject.AddLineBreak();
			pSubject.AddText("Realizacja darowizn");
			pSubject.AddLineBreak();
			pSubject.AddText($"ID: {M.InstanceInfo.ID}, Data: {M.InstanceInfo.DateReal:dd.MM.yyyy}");
			pSubject.Format.Alignment = ParagraphAlignment.Left;
			pSubject.Format.Font.Size = 12;
			pSubject.Format.Font.Color = color;
			pSubject.Format.Font.Bold = true;

			///Content
			Table tContent = section.AddTable();
			tContent.Borders.Width = 0.75;
			Column column = tContent.AddColumn("2cm");
			column.Format.Alignment = ParagraphAlignment.Center;

			column = tContent.AddColumn("3.5cm");
			column.Format.Alignment = ParagraphAlignment.Right;

			column = tContent.AddColumn("3.5cm");
			column.Format.Alignment = ParagraphAlignment.Right;

			column = tContent.AddColumn("3.5cm");
			column.Format.Alignment = ParagraphAlignment.Right;

			column = tContent.AddColumn("3.5cm");
			column.Format.Alignment = ParagraphAlignment.Right;

			///Table header
			Row row = tContent.AddRow();
			row.HeadingFormat = true;
			row.Format.Alignment = ParagraphAlignment.Center;
			row.Shading.Color = lColor;
			row.Format.Font.Bold = true;
			row.Format.Font.Size = 11;

			row.Cells[0].AddParagraph("Nr:");
			row.Cells[1].AddParagraph("Rodzina:");
			row.Cells[2].AddParagraph("Waga\ndarowizny (kg):");
			row.Cells[3].AddParagraph("Data\nodebrania:");
			row.Cells[4].AddParagraph("Podpis\nodbierającego:");

			///Table content
			Row row1 = null;
			int index = 0;
			foreach (var family in M.InstanceInfo.Families)
			{
				var amount = 0d;
				foreach (DataRow rValue in family.Positions.Rows)
					if (rValue.RowState != DataRowState.Deleted)
						amount += Convert.ToDouble(rValue["amount"]) * SQL.GetArtDefMeaCon((int)rValue["article"]);

				index++;
				row1 = tContent.AddRow();
				row1.Format.Alignment = ParagraphAlignment.Center;
				row1.Shading.Color = Color.Empty;
				row1.Format.Font.Bold = false;
				row1.TopPadding = 10;
				row1.BottomPadding = 10;
				row1.Format.Font.Size = 11;
				row1.Cells[0].AddParagraph(index.ToString());
				row1.Cells[1].AddParagraph($"{family.FamilyName}");
				row1.Cells[2].AddParagraph($"{amount}");
				if (family.Status == (short)C_DistributionFamily.DistributionFamilyStatus.Taken)
					row1.Cells[4].AddParagraph("Odebrano");
			}

			///Render
			PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
			renderer.Document = document;
			renderer.RenderDocument();

			///Save
			string filename = "ListaRealizacji.pdf";
			renderer.PdfDocument.Save(filename);

			try
			{
				///Open document.pdf
				Process.Start(filename);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
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
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_DistributionsAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.Module.DISTRIBUTIONS;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private C_Distribution instanceInfo;
		public C_Distribution InstanceInfo
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
					return "Nowa dystrybucja";
				else if (EditMode)
					return $"Edycja dystrybucji: {(string.IsNullOrEmpty(InstanceInfo.Name) ? InstanceInfo.ID.ToString() : InstanceInfo.Name)} - {InstanceInfo.DateReal:yyyy-MM-dd}";
				else
					return $"Podgląd dystrybucji: {(string.IsNullOrEmpty(InstanceInfo.Name) ? InstanceInfo.ID.ToString() : InstanceInfo.Name)} - {InstanceInfo.DateReal:yyyy-MM-dd}";
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
