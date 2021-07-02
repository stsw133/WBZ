using StswExpress;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Modules._base;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for AttributesTab.xaml
    /// </summary>
    public partial class AttributesTab : UserControl
    {
        readonly D_AttributesTab D = new D_AttributesTab();

        private MV Module;
        private int InstanceID;
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
                var win = Window.GetWindow(this);
                var d = win?.DataContext as D_ModuleNew<dynamic>;

                if (d != null)
                {
                    Module = d.Module;
                    InstanceID = (d.InstanceData as M).ID;
                }
                if (InstanceID != 0 && D.InstanceAttributes == null)
                    D.InstanceAttributes = SQL.ListAttributes(Module, InstanceID);
            }
            catch { }
        }

        /// <summary>
        /// Change
        /// </summary>
        private void BtnAttributeChange_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            var dataGrid = Content as DataGrid;

            if (d != null)
                EditingMode = (bool)(d.Mode != Commands.Type.PREVIEW);

            var indexes = dataGrid.SelectedItems.Cast<M_Attribute>().Select(x => D.InstanceAttributes.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new AttributeChange(D.InstanceAttributes[index], EditingMode);
                window.Owner = win;
                if (window.ShowDialog() == true)
                    SQL.UpdateAttribute(Module, D.InstanceAttributes[index]);
            }

            D.InstanceAttributes = SQL.ListAttributes(Module, InstanceID);
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    internal class D_AttributesTab : D
    {
        /// Module


        /// Attributes
        private List<M_Attribute> instanceAttributes;
        public List<M_Attribute> InstanceAttributes
        {
            get => instanceAttributes;
            set => SetField(ref instanceAttributes, value, () => InstanceAttributes);
        }
    }
}
