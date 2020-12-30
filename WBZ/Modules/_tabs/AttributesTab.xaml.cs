using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WBZ.Controls;
using WBZ.Models;

namespace WBZ.Modules._tabs
{
    /// <summary>
    /// Interaction logic for AttributesTab.xaml
    /// </summary>
    public partial class AttributesTab : UserControl
    {
        D_AttributesTab D = new D_AttributesTab();
        private string Module;
        private int ID;
        private bool EditingMode;

        public AttributesTab()
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

                if (ID != 0 && D.InstanceAttributes == null)
                    D.InstanceAttributes = SQL.ListAttributes(Module, ID);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                }
            }
            catch { }
        }

        /// <summary>
        /// Change
        /// </summary>
        private void btnAttributeChange_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = Content as System.Windows.Controls.DataGrid;
            Window win = Window.GetWindow(this);

            dynamic d = win?.DataContext;
            if (d != null)
                EditingMode = (bool)d.EditingMode;

            var indexes = dataGrid.SelectedItems.Cast<M_Attribute>().Select(x => D.InstanceAttributes.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new AttributeChange(D.InstanceAttributes[index], EditingMode);
                window.Owner = win;
                if (window.ShowDialog() == true)
                    SQL.UpdateAttribute(D.InstanceAttributes[index]);
            }

            D.InstanceAttributes = SQL.ListAttributes(Module, ID);
        }
	}

	/// <summary>
	/// DataContext
	/// </summary>
	class D_AttributesTab : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// Attributes
        private List<M_Attribute> instanceAttributes;
        public List<M_Attribute> InstanceAttributes
        {
            get
            {
                return instanceAttributes;
            }
            set
            {
                instanceAttributes = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
    }
}
