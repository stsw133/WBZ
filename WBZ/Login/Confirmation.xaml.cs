using System.Windows;

namespace WBZ.Login
{
	/// <summary>
	/// Interaction logic for Confirmation.xaml
	/// </summary>
	public partial class Confirmation : Window
	{
		public Confirmation()
		{
			InitializeComponent();
		}

		public string GetLogin => TxtBoxLogin.Text;
		public string GetPassword => PwdBoxPassword.Password;

		/// <summary>
		/// Accept
		/// </summary>
		private void BtnAccept_Click(object sender, RoutedEventArgs e) => DialogResult = true;

		/// <summary>
		/// Cancel
		/// </summary>
		private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
	}
}
