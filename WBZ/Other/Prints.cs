using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Diagnostics;
using System.Windows;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Other
{
	internal static class Prints
	{
        /// <summary>
        /// RODO
        /// </summary>
		internal static void Print_RODO(C_Family family, C_Contact contact)
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
                document.Info.Author = $"{Global.Database.Name}";

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
                pSubject.AddText($"{Global.Database.Name}");
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
	}
}
