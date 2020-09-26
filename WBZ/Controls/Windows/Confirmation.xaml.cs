using System.Windows;
using System.Windows.Input;

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

		public string GetLogin
		{
			get
			{
				return tbLogin.Text;
			}
		}
		public string GetPassword
		{
			get
			{
				return pbPassword.Password;
			}
		}

		/// <summary>
		/// Accept
		/// </summary>
		private void btnAccept_Click(object sender, MouseButtonEventArgs e)
		{
			DialogResult = true;
		}
	}
}
