using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using WBZ.Classes;

namespace WBZ.Helpers
{
	/// <summary>
	/// Interaction logic for AttributeValueChange.xaml
	/// </summary>
	public partial class AttributeValueChange : Window
	{
		M_AttributeValueChange M = new M_AttributeValueChange();

		public AttributeValueChange(C_Attribute attribute, bool editMode)
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
		/// Atrybut
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
		/// Wartości atrybutu
		public string[] AttributeValues { get { return AttributeInfo.Class.Values.Split(';'); } }
		/// Tryb wyboru wartości
		public bool FreeValues { get { return string.IsNullOrEmpty(AttributeInfo.Class.Values); } }
		/// Tryb edycji dla okna
		public bool EditMode { get; set; }
		/// Tytuł okna
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
