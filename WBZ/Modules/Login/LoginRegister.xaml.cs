using System.Windows;
using System.Windows.Input;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for LoginRegister.xaml
	/// </summary>
	public partial class LoginRegister : Window
	{
		public LoginRegister()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Przycisk akceptacji
		/// </summary>
		private void btnAccept_Click(object sender, MouseButtonEventArgs e)
		{
			if (SQL.Register(tbEmail.Text, tbUsername.Text, pbPassword.Password, true))
				DialogResult = true;
		}
	}
}
