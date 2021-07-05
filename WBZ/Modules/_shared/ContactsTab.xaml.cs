using StswExpress;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for ContactsTab.xaml
    /// </summary>
    public partial class ContactsTab : UserControl
    {
        readonly D_ContactsTab D = new D_ContactsTab();

        public ContactsTab()
        {
            InitializeComponent();
            //DataContext = D;
        }

        /// <summary>
        /// Module
        /// </summary>
        public static readonly DependencyProperty ModuleProperty
            = DependencyProperty.Register(
                  nameof(Module),
                  typeof(MV),
                  typeof(ContactsTab),
                  new PropertyMetadata(default(MV))
              );
        public MV Module
        {
            get => (MV)GetValue(ModuleProperty);
            set => SetValue(ModuleProperty, value);
        }

        /// <summary>
        /// InstanceID
        /// </summary>
        public static readonly DependencyProperty InstanceIDProperty
            = DependencyProperty.Register(
                  nameof(InstanceID),
                  typeof(int),
                  typeof(ContactsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InstanceID > 0 && D.InstanceContacts == null)
                    //D.InstanceContacts = SQL.ListInstances<M_Log>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
                    D.InstanceContacts = SQL.ListContacts(Module, InstanceID);
                DataContext = D;
                Loaded -= UserControl_Loaded;
                Window.GetWindow(this).Closed += UserControl_Closed;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd inicjalizacji zakładki kontaktów", ex, Module, InstanceID);
            }
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void UserControl_Closed(object sender, EventArgs e)
        {
            try
            {
                SQL.UpdateContacts(Module, InstanceID, D.InstanceContacts);
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd aktualizacji kontaktów", ex, Module, InstanceID);
            }
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    internal class D_ContactsTab : D
    {
        /// Module
        //public MV Module = Config.GetModule(nameof(Contacts));

        /// Contacts
        private DataTable instanceContacts;
        public DataTable InstanceContacts
        {
            get => instanceContacts;
            set => SetField(ref instanceContacts, value, () => InstanceContacts);
        }
    }
}