using StswExpress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for AttributesTab.xaml
    /// </summary>
    public partial class AttributesTab : UserControl
    {
        readonly D_AttributesTab D = new D_AttributesTab();

        public AttributesTab()
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
                  typeof(AttributesTab),
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
                  typeof(AttributesTab),
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
                if (InstanceID > 0 && D.InstanceAttributes == null)
                    //D.InstanceAttributes = SQL.ListInstances<M_Log>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
                    D.InstanceAttributes = SQL.ListAttributes(Module, InstanceID);
                DataContext = D;
                Loaded -= UserControl_Loaded;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd inicjalizacji zakładki atrybutów", ex, Module, InstanceID);
            }
        }

        /// <summary>
        /// Change
        /// </summary>
        private void BtnAttributeChange_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            var dataGrid = Content as DataGrid;

            //if (d != null)
            //    EditingMode = (bool)(d.Mode != Commands.Type.PREVIEW);

            var indexes = dataGrid.SelectedItems.Cast<M_Attribute>().Select(x => D.InstanceAttributes.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new AttributeChange(D.InstanceAttributes[index], /*EditingMode*/ true);
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
        //public MV Module = Config.GetModule(nameof(Attributes));

        /// Attributes
        private List<M_Attribute> instanceAttributes;
        public List<M_Attribute> InstanceAttributes
        {
            get => instanceAttributes;
            set => SetField(ref instanceAttributes, value, () => InstanceAttributes);
        }
    }
}
