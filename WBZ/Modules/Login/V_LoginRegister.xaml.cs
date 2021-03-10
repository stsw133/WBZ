using System.Windows;
using WBZ.Controls;

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
		/// Accept
		/// </summary>
		private void btnAccept_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(pbPassword.Password))
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nie podano nowego hasła!") { Owner = this }.ShowDialog();
				return;
			}
			else if (pbPassword.Password != pbRepass.Password)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nowe hasło nie zgadza się z powtórzeniem!") { Owner = this }.ShowDialog();
				return;
			};

			if (SQL.Register(tbEmail.Text, tbUsername.Text, pbPassword.Password, true))
				DialogResult = true;
		}
	}
}
