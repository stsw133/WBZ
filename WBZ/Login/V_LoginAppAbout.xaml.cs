using System.Windows;

namespace WBZ.Login
{
	/// <summary>
	/// Interaction logic for LoginAppAbout.xaml
	/// </summary>
	public partial class LoginAppAbout : Window
	{
		D_LoginAppAbout D = new D_LoginAppAbout();

		public LoginAppAbout()
		{
			InitializeComponent();
			DataContext = D;
		}
	}
}
