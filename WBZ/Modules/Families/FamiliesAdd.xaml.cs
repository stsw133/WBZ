using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using WBZ.Modules.Distributions;
using System.Reflection;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyAdd.xaml
    /// </summary>
    public partial class FamiliesAdd : Window
    {
        M_FamiliesAdd M = new M_FamiliesAdd();

        public FamiliesAdd(C_Family instance, bool editMode)
        {
            InitializeComponent();
            DataContext = M;

            M.InstanceInfo = instance;
            M.EditMode = editMode;
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

            if (SQL.SetFamily(M.InstanceInfo))
                Close();
        }
        private void btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn != null)
                btn.ContextMenu.IsOpen = true;
        }
        private void btnRodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                const string rodo = "Wyrażam zgodę na przetwarzanie moich danych osobowych dla potrzeb niezbędnych do realizacji procesu rejestracji" +
                        " (zgodnie z ustawą z dnia 10 maja 2018 roku o ochronie danych osobowych (Dz. Ustaw z 2018, poz. 1000) oraz" +
                        " zgodnie z Rozporządzeniem Parlamentu Europejskiego i Rady (UE) 2016/679 z dnia 27 kwietnia 2016 r. w sprawie ochrony osób fizycznych w związku" +
                        " z przetwarzaniem danych osobowych i w sprawie swobodnego przepływu takich danych oraz uchylenia dyrektywy 95/46/WE (RODO)).";
                MigraDoc.DocumentObjectModel.Color color = MigraDoc.DocumentObjectModel.Color.FromRgb(0, 0, 0);
                MigraDoc.DocumentObjectModel.Color lColor = MigraDoc.DocumentObjectModel.Color.FromRgb(192, 192, 192);

                Document document = new Document();
                document.Info.Title = "Kwestionariusz dodania rodziny";
                document.Info.Subject = $"Dodawanie rodziny: {M.InstanceInfo.Lastname}";
                document.Info.Author = $"{Global.Database.Name}";

                ///Title
                Section section = document.AddSection();
                section.PageSetup.TopMargin = "4cm";
                section.PageSetup.BottomMargin = "4cm";

                MigraDoc.DocumentObjectModel.Shapes.Image img = section.Headers.Primary.AddImage("Resources/logo.png");
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
                pSubject.AddText($"Rodzina: {M.InstanceInfo.Lastname}");
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
                pContent.AddText($"Zgłaszający: {M.InstanceInfo.Declarant}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Rodzina: {M.InstanceInfo.Lastname}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Ilość osób: {M.InstanceInfo.Members}");
                pContent.AddLineBreak();
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Adres: {M.InstanceInfo.Address}");
                pContent.AddLineBreak();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddTab();
                pContent.AddText($"Kod pocztowy: {M.InstanceInfo.Postcode}");
                pContent.AddLineBreak();
                pContent.AddLineBreak();
                if (M.InstanceInfo.Contacts.Select("default = true").Length > 0)
                {
                    if (M.InstanceInfo.Contacts.Select("default = true")[0]["phone"].ToString().Length > 0)
                    {
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddText($"Numer telefonu: {M.InstanceInfo.Contacts.Select("default = true")[0]["phone"]}");
                        pContent.AddLineBreak();
                    }
                    if (M.InstanceInfo.Contacts.Select("default = true")[0]["email"].ToString().Length > 0)
                    {
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddTab();
                        pContent.AddText($"Adres e-mail: {M.InstanceInfo.Contacts.Select("default = true")[0]["email"]}");
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
                var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
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

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name == "tabSources_Distributions")
            {
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Distributions == null)
					M.InstanceSources_Distributions = SQL.ListDistributions($"dp.family={M.InstanceInfo.ID}");
            }
        }

        private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}"))
                {
                    var indexes = (sender as DataGrid).SelectedItems.Cast<C_Distribution>().Select(x => M.InstanceSources_Distributions.IndexOf(x));
                    foreach (int index in indexes)
                    {
                        var window = new DistributionsAdd(M.InstanceSources_Distributions[index], true);
                        window.Show();
                    }
                }
                else if (Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.PREVIEW}"))
                {
                    var indexes = (sender as DataGrid).SelectedItems.Cast<C_Distribution>().Select(x => M.InstanceSources_Distributions.IndexOf(x));
                    foreach (int index in indexes)
                    {
                        var window = new DistributionsAdd(M.InstanceSources_Distributions[index], false);
                        window.Show();
                    }
                }
            }
        }
    }

    /// <summary>
	/// Model
	/// </summary>
	internal class M_FamiliesAdd : INotifyPropertyChanged
    {
        public readonly string INSTANCE_TYPE = Global.Module.FAMILIES;

        /// Dane o zalogowanym użytkowniku
        public C_User User { get; } = Global.User;
        /// Instancja
        private C_Family instanceInfo;
        public C_Family InstanceInfo
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
        /// Źródło instancji - dystrybucje
        private List<C_Distribution> instanceSources_Distributions;
        public List<C_Distribution> InstanceSources_Distributions
        {
            get
            {
                return instanceSources_Distributions;
            }
            set
            {
                instanceSources_Distributions = value;
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
                    return "Nowa rodzina";
                else if (EditMode)
                    return $"Edycja rodziny: {InstanceInfo.Lastname}";
                else
                    return $"Podgląd rodziny: {InstanceInfo.Lastname}";
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
