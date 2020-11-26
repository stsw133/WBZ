using System;
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
			D.EditMode = editMode;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if (D.AttributeInfo.Value == null)
				D.AttributeInfo.Value = "";

			if ((string.IsNullOrEmpty(D.AttributeInfo.Value))
			||  (D.AttributeInfo.Class.Type == "char" && char.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "date" && DateTime.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "double" && double.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "int" && int.TryParse(D.AttributeInfo.Value, out _))
			||  (D.AttributeInfo.Class.Type == "string"))
				DialogResult = true;
			else
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"Wartość niezgodna z typem ({D.AttributeInfo.Class.Type}) danych klasy atrybutu!") { Owner = this }.ShowDialog();
		}
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	public class D_AttributeValueChange : INotifyPropertyChanged
	{
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
		public string[] AttributeValues { get { return AttributeInfo.Class.Values.Split(';'); } }
		/// Can attribute have any value
		public bool FreeValues { get { return string.IsNullOrEmpty(AttributeInfo.Class.Values); } }
		/// Edit mode
		public bool EditMode { get; set; }
		/// Window title
		public string Title
		{
			get
			{
				if (string.IsNullOrEmpty(AttributeInfo.Value))
					return $"Wartość nowego atrybutu: {AttributeInfo.Class.Name}";
				else if (EditMode)
					return $"Edycja wartości atrybutu: {AttributeInfo.Class.Name}";
				else
					return $"Podgląd wartości atrybutu: {AttributeInfo.Class.Name}";
			}
		}

		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
