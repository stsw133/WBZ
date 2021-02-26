using StswExpress.Globals;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using WBZ.Models;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for AttributeChange.xaml
	/// </summary>
	public partial class AttributeChange : Window
	{
		D_AttributeValueChange D = new D_AttributeValueChange();

		public AttributeChange(M_Attribute attribute, bool editMode)
		{
			InitializeComponent();
			DataContext = D;

			D.AttributeInfo = attribute;
			D.AttributeValues = SQL.ComboInstances("attributes_values", "value", $"class={attribute.Class.ID} and archival=false", true);
			D.EditMode = editMode;
		}

		/// <summary>
		/// OK
		/// </summary>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if (D.AttributeInfo.Value == null)
				D.AttributeInfo.Value = string.Empty;

			if ((string.IsNullOrEmpty(D.AttributeInfo.Value))
			||  (D.AttributeInfo.Class.Type == "date" && DateTime.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "double" && double.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "int" && int.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type.In("string", "list")))
				DialogResult = true;
			else
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"Wartość niezgodna z typem ({D.AttributeInfo.Class.Type}) danych klasy atrybutu!") { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Cancel
		/// </summary>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}

	/// <summary>
	/// DataContext
	/// </summary>
	public class D_AttributeValueChange : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Window title
		public string Title
		{
			get
			{
				if (string.IsNullOrEmpty(AttributeInfo.Value))
					return $"Wartość nowego atrybutu";
				else if (EditMode)
					return $"Edycja wartości atrybutu";
				else
					return $"Podgląd wartości atrybutu";
			}
		}
		/// Attribute
		private M_Attribute attributeInfo;
		public M_Attribute AttributeInfo
		{
			get
			{
				return attributeInfo;
			}
			set
			{
				attributeInfo = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Attribute values
		private ObservableCollection<M_ComboValue> attributeValues;
		public ObservableCollection<M_ComboValue> AttributeValues
		{
			get
			{
				return attributeValues;
			}
			set
			{
				attributeValues = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Can attribute have any value
		public bool FreeValues { get { return AttributeInfo.Class.Type != "list"; } }
		/// Edit mode
		public bool EditMode { get; set; }
	}
}
