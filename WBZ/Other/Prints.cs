using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using StswExpress.Globals;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using WBZ.Models;

namespace WBZ.Other
{
    internal static class Prints
    {
        //TODO - ogarnięcie nowego sposobu na wydruki w wersji 1.4.0
        /// <summary>
        /// RODO
        /// </summary>
		internal static void Print_RODO(M_Family family, M_Contact contact)
        {
            try
            {
                const string rodo = @"Wyrażam zgodę na przetwarzanie moich danych osobowych dla potrzeb niezbędnych do realizacji procesu rejestracji
(zgodnie z ustawą z dnia 10 maja 2018 roku o ochronie danych osobowych (Dz. Ustaw z 2018, poz. 1000) oraz
zgodnie z Rozporządzeniem Parlamentu Europejskiego i Rady (UE) 2016/679 z dnia 27 kwietnia 2016 r. w sprawie ochrony osób fizycznych w związku
z przetwarzaniem danych osobowych i w sprawie swobodnego przepływu takich danych oraz uchylenia dyrektywy 95/46/WE (RODO)).";
                Color color = Color.FromRgb(0, 0, 0);
                Color lColor = Color.FromRgb(192, 192, 192);

                Document document = new Document();
                document.Info.Title = "Kwestionariusz dodania rodziny";
                document.Info.Subject = $"Dodawanie rodziny: {family.Lastname}";
                document.Info.Author = $"{Global.AppDatabase.Name}";

                ///Title
                Section section = document.AddSection();
                section.PageSetup.TopMargin = "4cm";
                section.PageSetup.BottomMargin = "4cm";

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
                pSubject.AddText($"{Global.AppDatabase.Name}");
                pSubject.AddLineBreak();
                pSubject.AddLineBreak();
                pSubject.AddText("Dodawanie rodziny");
                pSubject.AddLineBreak();
                pSubject.AddText($"Rodzina: {family.Lastname}");
                pSubject.AddLineBreak();
                pSubject.AddDateField("dd.MM.yyyy");
                pSubject.Format.Alignment = ParagraphAlignment.Left;
                pSubject.Format.Font.Size = 12;
                pSubject.Format.Font.Color = color;
                pSubject.Format.Font.Bold = true;

                ///Content
                Paragraph pContent = section.AddParagraph();
                for (int x = 0; x < 7; x++)
                    pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Zgłaszający: {family.Declarant}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Rodzina: {family.Lastname}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Ilość osób: {family.Members}");
                pContent.AddLineBreak();
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Adres: {family.Address}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Kod pocztowy: {family.Postcode}");
                pContent.AddLineBreak();
                pContent.AddLineBreak();
                if (contact != null)
                {
                    if (contact.Phone.Length > 0)
                    {
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddText($"Numer telefonu: {contact.Phone}");
                        pContent.AddLineBreak();
                    }
                    if (contact.Email.Length > 0)
                    {
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddText($"Adres e-mail: {contact.Email}");
                        pContent.AddLineBreak();
                    }
                }

                pContent.Format.Alignment = ParagraphAlignment.Left;
                pContent.Format.Font.Size = 18;
                pContent.Format.Font.Color = color;

                ///Rodo
                Table tRodo = section.Footers.Primary.AddTable();

                Column column = tRodo.AddColumn("12cm");
                column.Format.Alignment = ParagraphAlignment.Justify;

                column = tRodo.AddColumn("4cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = tRodo.AddRow();
                row.Cells[0].AddParagraph(rodo);
                row.Cells[1].AddParagraph("Podpis");
                row.Format.Font.Size = 12;
                row.Format.Font.Color = color;

                ///Render
                var renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                ///Save
                string filename = "DodawanieRodziny.pdf";
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ReportGenerateDonationsSum
        /// </summary>
		internal static void Print_ReportGenerateDonationsSum(DateTime dFrom, DateTime dTo)
        {
            string filename = "RaportSumaryczny.pdf";
            Color color = Color.FromRgb(0, 0, 0);
            Color lColor = Color.FromRgb(192, 192, 192);

            ///Create document.pdf
            Document document = new Document();
            document.Info.Title = $"Raport sumaryczny darowizn";
            document.Info.Subject = $"Raport sumaryczny darowizn {dFrom:dd.MM.yyyy} -> {dTo:dd.MM.yyyy}";
            document.Info.Author = $"{Global.AppDatabase.Name}";
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
            pSubject.AddText($"{Global.AppDatabase.Name}");
            pSubject.AddLineBreak();
            pSubject.AddLineBreak();
            pSubject.AddText("Raport sumaryczny darowizn");
            pSubject.AddLineBreak();
            pSubject.AddText($"{dFrom:yyyy.MM.dd} - {dTo:yyyy.MM.dd}");
            document.Info.Author = $"{Global.AppDatabase.Name}";
            pSubject.Format.Alignment = ParagraphAlignment.Left;
            pSubject.Format.Font.Size = 11;
            pSubject.Format.Font.Color = color;
            pSubject.Format.Font.Bold = false;

            ///Content
            var dtContractors = SQL.GetDonationSumContractor(dFrom, dTo);

            float columnWidth = ((float)document.DefaultPageSetup.PageHeight - (float)document.DefaultPageSetup.LeftMargin
                - (float)document.DefaultPageSetup.RightMargin) / (2 * dtContractors.Rows.Count + (4 * 2));

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
            foreach (DataRow drRow in dtContractors.Rows)
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
            foreach (DataRow drRow in dtContractors.Rows)
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

            foreach (DataRow drRow in dtContractors.Rows)
            {
                row.Cells[gCells].AddParagraph("kg");
                row.Cells[++gCells].AddParagraph("zł");
                gCells++;
            }
            ///Table Rows(content)
            var dtDate = SQL.GetDonationSumDate(dFrom, dTo);

            Row row1 = null;
            int nr = 1;
            float[] sumAll = new float[dtContractors.Rows.Count * 2 + 2];
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
                foreach (DataRow dataRow in dtContractors.Rows)
                {
                    var drContractorsValue = SQL.GetDonationSumContractorValue((DateTime)drRow["dateissue"], dataRow["name"].ToString());

                    if (drContractorsValue["amount"].ToString().Length == 0)
                        row1.Cells[cells].AddParagraph("0");
                    else
                        row1.Cells[cells].AddParagraph(drContractorsValue["amount"].ToString());

                    if (drContractorsValue["cost"].ToString().Length == 0)
                        row1.Cells[++cells].AddParagraph("0");
                    else
                        row1.Cells[++cells].AddParagraph(drContractorsValue["cost"].ToString());
                    cells++;

                    if (drContractorsValue["amount"].ToString().Length != 0)
                    {
                        sumAmount += float.Parse(drContractorsValue["amount"].ToString());
                        sumAll[index] += float.Parse(drContractorsValue["amount"].ToString());
                    }
                    index++;

                    if (drContractorsValue["cost"].ToString().Length != 0)
                    {
                        sumCost += float.Parse(drContractorsValue["cost"].ToString());
                        sumAll[index] += float.Parse(drContractorsValue["cost"].ToString());
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
            foreach (DataRow dataRow in dtContractors.Rows)
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
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
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

        /// <summary>
        /// DistributionList
        /// </summary>
		internal static void Print_DistributionList(M_Distribution distribution)
        {
            Color color = Color.FromRgb(0, 0, 0);
            Color lColor = Color.FromRgb(192, 192, 192);

            ///Create document.pdf
            Document document = new Document();
            document.Info.Title = $"Realizacja darowizn";
            document.Info.Subject = $"Realizacja darowizn: {distribution.ID} - {distribution.DateReal}";
            document.Info.Author = $"{Global.AppDatabase.Name}";

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
            pSubject.AddText($"{Global.AppDatabase.Name}");
            pSubject.AddLineBreak();
            pSubject.AddLineBreak();
            pSubject.AddText("Realizacja darowizn");
            pSubject.AddLineBreak();
            pSubject.AddText($"ID: {distribution.ID}, Data: {distribution.DateReal:dd.MM.yyyy}");
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
            foreach (var family in distribution.Families)
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
                if (family.Status == (short)M_DistributionFamily.DistributionFamilyStatus.Taken)
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
    }
}
