using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using WBZ.Helpers;
using Color = MigraDoc.DocumentObjectModel.Color;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;

namespace WBZ.Modules.Stats
{
    public enum StatsReports
    {
        DonationsSum = 0
    }

    /// <summary>
    /// Interaction logic for StatsReportsGenerator.xaml
    /// </summary>
    public partial class StatsReportsGenerator : Window
    {
        private StatsReports type;
        private DataTable dtCompanies,dtDate = null;
        private DataRow drCompaniesValue = null;
        private DateTime from, to;

        public StatsReportsGenerator(StatsReports type)
        {
            InitializeComponent();
            this.type = type;
            dpFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpTo.SelectedDate = DateTime.Now;
        }

        private void btnAccept_Click(object sender, MouseButtonEventArgs e)
        {
            if (dpFrom.SelectedDate.HasValue && dpTo.SelectedDate.HasValue)
            {
                if (dpFrom.SelectedDate.Value > dpTo.SelectedDate.Value)
                {
                    MessageBox.Show("Błędny dobór daty!", "Kontrola składni!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    from = dpFrom.SelectedDate.Value.Date;
                    to = dpTo.SelectedDate.Value.Date;
                    switch (type)
                    {
                        case StatsReports.DonationsSum: 
                            ReportGenerateDonationsSum();
                            break;
                    }
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Ustawienie przedziału czasowego jest niezbędne do wygenerowania raportu!", "Kontrola składni!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ReportGenerateDonationsSum()
        {
            string filename = "RaportSumaryczny.pdf";
            Color color = Color.FromRgb(0, 0, 0);
            Color lColor = Color.FromRgb(192, 192, 192);

            ///Create document.pdf
            Document document = new Document();
            document.Info.Title = $"Raport sumaryczny darowizn";
            document.Info.Subject = $"Raport sumaryczny darowizn {dpFrom.SelectedDate.Value:dd.MM.yyyy} -> {dpTo.SelectedDate.Value:dd.MM.yyyy}";
            document.Info.Author = $"{Global.Database.Name}";
            document.DefaultPageSetup.Orientation = Orientation.Landscape;

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
            pSubject.AddText("Raport sumaryczny darowizn");
            pSubject.AddLineBreak();
            pSubject.AddText($"{dpFrom.SelectedDate.Value:yyyy.MM.dd} - {dpTo.SelectedDate.Value:yyyy.MM.dd}");
            document.Info.Author = $"{Global.Database.Name}";
            pSubject.Format.Alignment = ParagraphAlignment.Left;
            pSubject.Format.Font.Size = 11;
            pSubject.Format.Font.Color = color;
            pSubject.Format.Font.Bold = false;

            ///Content
            dtCompanies = SQL.GetDonationSumCompany(from, to);
            
            float columnWidth = ((float)document.DefaultPageSetup.PageHeight - (float)document.DefaultPageSetup.LeftMargin
                - (float)document.DefaultPageSetup.RightMargin)/ (2*dtCompanies.Rows.Count + (4*2));
            
            Table tContent = section.AddTable();
            tContent.Borders.Width = 0.75;
            ///Table Columns
            ///Lp.
            Column column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            ///Date
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            ///Amount
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            ///SumDayCost
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tContent.AddColumn(columnWidth);
            column.Format.Alignment = ParagraphAlignment.Center;
            ///GeneretColumns
            foreach (DataRow drRow in dtCompanies.Rows)
            {
                column = tContent.AddColumn(columnWidth);
                column.Format.Alignment = ParagraphAlignment.Center;
                column = tContent.AddColumn(columnWidth);
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            ///Table Rows(Header)
            Row row = tContent.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Shading.Color = lColor;
            row.Format.Font.Bold = true;

            int cells = 0;
            int gCells = 0;
            int index = 0;
            row.Cells[cells].AddParagraph("Lp.");
            row.Cells[cells].MergeDown = 1;
            row.Cells[cells].MergeRight = 1;
            cells += 2;
            row.Cells[cells].AddParagraph("Data");
            row.Cells[cells].MergeDown = 1;
            row.Cells[cells].MergeRight = 1;
            cells += 2;
            gCells = cells;
            foreach (DataRow drRow in dtCompanies.Rows)
            {
                row.Cells[cells].AddParagraph($"{drRow["name"]}");
                row.Cells[cells].MergeRight = 1;
                cells += 2;
            }
            row.Cells[cells].AddParagraph("Suma\nw kg");
            row.Cells[cells].MergeDown = 1;
            row.Cells[cells].MergeRight = 1;
            cells += 2;
            row.Cells[cells].AddParagraph("Suma\nw zł");
            row.Cells[cells].MergeDown = 1;
            row.Cells[cells].MergeRight = 1;

            row = tContent.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Shading.Color = lColor;
            row.Format.Font.Bold = true;

            foreach (DataRow drRow in dtCompanies.Rows)
            {
                row.Cells[gCells].AddParagraph("kg");
                row.Cells[++gCells].AddParagraph("zł");
                gCells++;
            }
            ///Table Rows(content)
            dtDate = SQL.GetDonationSumDate(from, to);
            
            Row row1 = null;
            int nr = 1;
            float[] sumAll = new float[dtCompanies.Rows.Count * 2 + 2];
            Array.Clear(sumAll, 0, sumAll.Length);

            foreach (DataRow drRow in dtDate.Rows)
            {
                cells = 0;
                row1 = tContent.AddRow();
                row1.Format.Alignment = ParagraphAlignment.Center;
                row1.Shading.Color = Color.Empty;
                row1.Format.Font.Bold = false;
                ///Lp.
                row1.Cells[cells].AddParagraph(nr.ToString());
                row1.Cells[cells].MergeRight = 1;
                cells += 2;
                ///Date
                row1.Cells[cells].AddParagraph(drRow["day"].ToString());
                row1.Cells[cells].MergeRight = 1;
                cells += 2;
                ///Comapnies
                float sumAmount = 0;
                float sumCost = 0;
                index = 0;
                foreach(DataRow dataRow in dtCompanies.Rows)
                {
                    drCompaniesValue = SQL.GetDonationSumCompanyValue((DateTime)drRow["dateissue"], dataRow["name"].ToString());

                    if(drCompaniesValue["amount"].ToString().Length == 0)
                        row1.Cells[cells].AddParagraph("0");
                    else
                        row1.Cells[cells].AddParagraph(drCompaniesValue["amount"].ToString());

                    if (drCompaniesValue["cost"].ToString().Length == 0)
                        row1.Cells[++cells].AddParagraph("0");
                    else
                        row1.Cells[++cells].AddParagraph(drCompaniesValue["cost"].ToString());
                    cells++;

                    if (drCompaniesValue["amount"].ToString().Length != 0)
                    {
                        sumAmount += float.Parse(drCompaniesValue["amount"].ToString());
                        sumAll[index] += float.Parse(drCompaniesValue["amount"].ToString());
                    }
                    index++;

                    if (drCompaniesValue["cost"].ToString().Length != 0)
                    {
                        sumCost += float.Parse(drCompaniesValue["cost"].ToString());
                        sumAll[index] += float.Parse(drCompaniesValue["cost"].ToString());
                    }
                    index++;
                }

                sumAll[index] += sumAmount;
                row1.Cells[cells].AddParagraph($"{sumAmount}");
                row1.Cells[cells].MergeRight = 1;
                cells += 2;
                index++;

                sumAll[index] += sumCost;
                row1.Cells[cells].AddParagraph($"{sumCost}");
                row1.Cells[cells].MergeRight = 1;
                nr++;
            }

            ///Table Rows(sum)
            Row rSum = tContent.AddRow();
            rSum.Format.Alignment = ParagraphAlignment.Center;
            rSum.Shading.Color = lColor;
            rSum.Format.Font.Bold = true;
            ///sum
            cells = 0;
            rSum.Cells[cells].AddParagraph("RAZEM");
            rSum.Cells[cells].MergeRight = 3;
            cells += 4;

            index = 0;
            foreach (DataRow dataRow in dtCompanies.Rows)
            {
                rSum.Cells[cells].AddParagraph(sumAll[index].ToString());
                rSum.Cells[++cells].AddParagraph(sumAll[++index].ToString());
                index++;
                cells++;
            }

            rSum.Cells[cells].AddParagraph(sumAll[index].ToString());
            rSum.Cells[cells].MergeRight = 1;
            cells += 2;
            index++;

            rSum.Cells[cells].AddParagraph(sumAll[index].ToString());
            rSum.Cells[cells].MergeRight = 1;

            ///Render
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();

            ///Save
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
    }
}
