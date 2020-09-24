using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using WBZ.Modules.Documents;

namespace WBZ.Modules.Companies
{
    /// <summary>
    /// Logika interakcji dla klasy CompaniesAdd.xaml
    /// </summary>
    public partial class CompaniesAdd : Window
    {
        M_CompaniesAdd M = new M_CompaniesAdd();

        public CompaniesAdd(C_Company instance, bool editMode)
        {
            InitializeComponent();
            DataContext = M;

            M.InstanceInfo = instance;
            M.InstanceInfo.Contacts = SQL.ListContacts(M.INSTANCE_TYPE, M.InstanceInfo.ID);
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

            if (SQL.SetCompany(M.InstanceInfo))
                Close();
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
            if (tab?.Name == "tabSources_Documents")
            {
				if (M.InstanceInfo.ID != 0 && M.InstanceSources_Documents == null)
					M.InstanceSources_Documents = SQL.ListDocuments($"c.id={M.InstanceInfo.ID}");
            }
        }

        private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}"))
                {
                    var indexes = (sender as DataGrid).SelectedItems.Cast<C_Document>().Select(x => M.InstanceSources_Documents.IndexOf(x));
                    foreach (int index in indexes)
                    {
                        var window = new DocumentsAdd(M.InstanceSources_Documents[index], true);
                        window.Show();
                    }
                }
                else if (Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.PREVIEW}"))
                {
                    var indexes = (sender as DataGrid).SelectedItems.Cast<C_Document>().Select(x => M.InstanceSources_Documents.IndexOf(x));
                    foreach (int index in indexes)
                    {
                        var window = new DocumentsAdd(M.InstanceSources_Documents[index], false);
                        window.Show();
                    }
                }
            }
        }
    }

    /// <summary>
	/// Model
	/// </summary>
	internal class M_CompaniesAdd : INotifyPropertyChanged
    {
        public readonly string INSTANCE_TYPE = Global.Module.COMPANIES;

        /// Dane o zalogowanym użytkowniku
        public C_User User { get; } = Global.User;
        /// Instancja
        private C_Company instanceInfo;
        public C_Company InstanceInfo
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
        /// Źródło instancji - dokumenty
        private List<C_Document> instanceSources_Documents;
        public List<C_Document> InstanceSources_Documents
        {
            get
            {
                return instanceSources_Documents;
            }
            set
            {
                instanceSources_Documents = value;
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
                    return "Nowa firma";
                else if (EditMode)
                    return $"Edycja firmy: {InstanceInfo.Name}";
                else
                    return $"Podgląd firmy: {InstanceInfo.Name}";
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
