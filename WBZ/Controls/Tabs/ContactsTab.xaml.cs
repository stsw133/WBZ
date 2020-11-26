using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for ContactsTab.xaml
    /// </summary>
    public partial class ContactsTab : UserControl
    {
        D_ContactsTab D = new D_ContactsTab();
        private string Module;
        private int ID;
        private bool EditingMode;

        public ContactsTab()
        {
            InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && D.InstanceContacts == null)
                {
                    D.InstanceContacts = SQL.ListContacts(Module, ID);
                    win.Closed += UserControl_Closed;
                }

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                    EditingMode = (bool)d.InstanceInfo.EditingMode;
                }
            }
            catch { }
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void UserControl_Closed(object sender, EventArgs e)
        {
            try
            {
                SQL.UpdateContacts(Module, ID, D.InstanceContacts);
            }
            catch { }
        }
    }

    /// <summary>
    /// Model
    /// </summary>
    internal class D_ContactsTab : INotifyPropertyChanged
    {
        /// Contacts
        private DataTable instanceContacts;
        public DataTable InstanceContacts
        {
            get
            {
                return instanceContacts;
            }
            set
            {
                instanceContacts = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
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