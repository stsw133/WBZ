using System.Windows;

namespace WBZ.Controls
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

		public string GetLogin => tbLogin.Text;
		public string GetPassword => pbPassword.Password;

		/// <summary>
		/// Accept
		/// </summary>
		private void btnAccept_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
