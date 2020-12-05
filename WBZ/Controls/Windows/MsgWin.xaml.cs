using System;
using System.Windows;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for MsgWin.xaml
	/// </summary>
	public partial class MsgWin : Window
	{
		public enum Type
		{
			MsgOnly = 0,
			InputBox = 1
		}
		public static class MsgTitle
		{
			public const string CONFIRMATION = "Potwierdzenie";
			public const string ERROR = "Błąd";
			public const string INFO = "Informacja";
			public const string WARNING = "Ostrzeżenie";
		}

		public MsgWin(Type type, string title = "", string message = "", string value = "")
		{
			InitializeComponent();

			Title = title;
			txtMessage.Text = message;
			tbInput.Text = value;

			///InputBox
			if (type == Type.InputBox)
				tbInput.Visibility = Visibility.Visible;
		}

		public string Value
		{
			get
			{
				return tbInput.Text;
			}
		}

		/// <summary>
		/// OK
		/// </summary>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		/// <summary>
		/// Cancel
		/// </summary>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
