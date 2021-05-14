using StswExpress;
using System.Collections.Generic;
using System.Linq;
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
                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.Module;
                    ID = (int)d.InstanceData.ID;
                }
                if (ID != 0 && D.InstanceAttributes == null)
                    D.InstanceAttributes = SQL.ListAttributes(Module, ID);
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
                EditingMode = (bool)(d.Mode != Commands.Type.PREVIEW);

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
	class D_AttributesTab : D
    {
        /// Attributes
        private List<M_Attribute> instanceAttributes;
        public List<M_Attribute> InstanceAttributes
        {
            get => instanceAttributes;
            set => SetField(ref instanceAttributes, value, () => InstanceAttributes);
        }
    }
}
