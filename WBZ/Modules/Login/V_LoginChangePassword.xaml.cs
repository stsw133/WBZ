using System.Windows;
using WBZ.Controls;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for LoginChangePassword.xaml
	/// </summary>
	public partial class LoginChangePassword : Window
	{
		public LoginChangePassword(string email)
		{
			InitializeComponent();
			lblEmail.Content = email;
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

			var user = SQL.ListInstances<M_User>(Global.Module.USERS, $"u.email='{lblEmail.Content}'")?[0];
			user.Newpass = pbPassword.Password;
			if (SQL.SetInstance(Global.Module.USERS, user, StswExpress.Globals.Commands.Type.EDIT))
				DialogResult = true;
		}
	}
}
