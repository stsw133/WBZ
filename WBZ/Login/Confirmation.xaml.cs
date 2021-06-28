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

		public string GetLogin => TxtBox_Login.Text;
		public string GetPassword => PwdBox_Password.Password;

		/// <summary>
		/// Accept
		/// </summary>
		private void btnAccept_Click(object sender, RoutedEventArgs e)
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
