using StswExpress.Base;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WBZ.Modules._tabs
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
                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                    EditingMode = (bool)d.EditingMode;
                }
                if (ID != 0 && D.InstanceContacts == null)
                {
                    D.InstanceContacts = SQL.ListContacts(Module, ID);
                    win.Closed += UserControl_Closed;
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
    /// DataContext
    /// </summary>
    class D_ContactsTab : D
    {
        /// Contacts
        private DataTable instanceContacts;
        public DataTable InstanceContacts
        {
            get => instanceContacts;
            set => SetField(ref instanceContacts, value, () => InstanceContacts);
        }
    }
}