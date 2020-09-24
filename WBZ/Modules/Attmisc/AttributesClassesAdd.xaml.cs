using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for AttributesClassesAdd.xaml
	/// </summary>
	public partial class AttributesClassesAdd : Window
	{
        M_AttributesClassesAdd M = new M_AttributesClassesAdd();

        public AttributesClassesAdd(C_AttributeClass instance, bool editMode)
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

            if (SQL.SetAttributeClass(M.InstanceInfo))
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
    }

    /// <summary>
	/// Model
	/// </summary>
	internal class M_AttributesClassesAdd : INotifyPropertyChanged
    {
        public readonly string INSTANCE_TYPE = Global.Module.ATTRIBUTES_CLASSES;

        /// Dane o zalogowanym użytkowniku
        public C_User User { get; } = Global.User;
        /// Instancja
        private C_AttributeClass instanceInfo;
        public C_AttributeClass InstanceInfo
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
                    return "Nowa klasa atrybutu";
                else if (EditMode)
                    return $"Edycja klasy atrybutu: {InstanceInfo.Name}";
                else
                    return $"Podgląd klasy atrybutu: {InstanceInfo.Name}";
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
