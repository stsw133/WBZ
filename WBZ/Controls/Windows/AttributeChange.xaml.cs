using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using WBZ.Classes;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for AttributeChange.xaml
	/// </summary>
	public partial class AttributeChange : Window
	{
		M_AttributeValueChange M = new M_AttributeValueChange();

		public AttributeChange(C_Attribute attribute, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.AttributeInfo = attribute;
			M.EditMode = editMode;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if (M.AttributeInfo.Value == null)
				M.AttributeInfo.Value = "";

			if ((string.IsNullOrEmpty(M.AttributeInfo.Value))
			||  (M.AttributeInfo.Class.Type == "char" && char.TryParse(M.AttributeInfo.Value, out _))
			||  (M.AttributeInfo.Class.Type == "date" && DateTime.TryParse(M.AttributeInfo.Value, out _))
			||  (M.AttributeInfo.Class.Type == "double" && double.TryParse(M.AttributeInfo.Value, out _))
			||  (M.AttributeInfo.Class.Type == "int" && int.TryParse(M.AttributeInfo.Value, out _))
			||  (M.AttributeInfo.Class.Type == "string"))
				DialogResult = true;
			else
				MessageBox.Show($"Wartość niezgodna z typem ({M.AttributeInfo.Class.Type}) danych klasy atrybutu!");
		}
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	public class M_AttributeValueChange : INotifyPropertyChanged
	{
		/// Attribute
		private C_Attribute attributeInfo;
		public C_Attribute AttributeInfo
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
