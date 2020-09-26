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

		public string[] values = new string[1];

		public MsgWin(Type type, string title = "", string message = "", string value = "")
		{
			InitializeComponent();

			Title = title;
			txtMessage.Content = message;
			tbInput.Text = value;

			///InputBox
			if (type == Type.InputBox)
				tbInput.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// OK
		/// </summary>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			values[0] = tbInput.Text;
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
