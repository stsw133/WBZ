using StswExpress;
using System;
using System.Collections.Generic;
using System.Windows;
using WBZ.Models;
using WBZ.Modules._base;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for AttributeChange.xaml
    /// </summary>
    public partial class AttributeChange : Window
    {
        readonly D_AttributeValueChange D = new D_AttributeValueChange();

        public AttributeChange(M_Attribute attribute, bool editMode)
        {
            InitializeComponent();
            DataContext = D;

            D.AttributeInfo = attribute;
            D.AttributeValues = SQL.ComboSource(new MV() { Tag = "atv" }, "value", $"class_id={attribute.Class.ID} and is_archival=false", true);
            D.EditMode = editMode;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (D.AttributeInfo.Value == null)
                D.AttributeInfo.Value = string.Empty;

            if ((string.IsNullOrEmpty(D.AttributeInfo.Value))
            || (D.AttributeInfo.Class.Type == "date" && DateTime.TryParse(D.AttributeInfo.Value, out _))
            || (D.AttributeInfo.Class.Type == "double" && double.TryParse(D.AttributeInfo.Value, out _))
            || (D.AttributeInfo.Class.Type == "int" && int.TryParse(D.AttributeInfo.Value, out _))
            || (D.AttributeInfo.Class.Type.In("string", "list")))
                DialogResult = true;
            else
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Wartość niezgodna z typem ({D.AttributeInfo.Class.Type}) danych klasy atrybutu!") { Owner = this }.ShowDialog();
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }

    /// <summary>
    /// DataContext
    /// </summary>
    public class D_AttributeValueChange : D
    {
        /// Window title
        public string Title
        {
            get
            {
                if      (string.IsNullOrEmpty(AttributeInfo.Value)) return $"Wartość nowego atrybutu";
                else if (EditMode)                                  return $"Edycja wartości atrybutu";
                else                                                return $"Podgląd wartości atrybutu";
            }
        }

        /// Attribute
        private M_Attribute attributeInfo;
        public M_Attribute AttributeInfo
        {
            get => attributeInfo;
            set => SetField(ref attributeInfo, value, () => AttributeInfo);
        }

        /// Attribute values
        private List<MV> attributeValues;
        public List<MV> AttributeValues
        {
            get => attributeValues;
            set => SetField(ref attributeValues, value, () => AttributeValues);
        }

        /// Can attribute have any value
        public bool FreeValues => AttributeInfo.Class.Type != "list";

        /// Edit mode
        public bool EditMode { get; set; }
    }
}
