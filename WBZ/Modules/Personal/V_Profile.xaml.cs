using System.Windows;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Globals;

namespace WBZ.Modules.Personal
{
	/// <summary>
	/// Interaction logic for Profile.xaml
	/// </summary>
	public partial class Profile : Window
	{
        D_Profile D = new D_Profile();

        public Profile()
		{
			InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// Save
        /// </summary>
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbOldPassword.Password) && !string.IsNullOrEmpty(tbNewPassword.Password) && !string.IsNullOrEmpty(tbRNewPassword.Password))
            {
                if (tbNewPassword.Password != tbRNewPassword.Password)
                {
                    new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nowe hasło nie zgadza się z powtórzeniem!") { Owner = this }.ShowDialog();
                    return;
                }
                else
                {
                    if (SQL.Login(Global.User.Username, Global.sha256(tbOldPassword.Password)))
                        D.User.Newpass = tbNewPassword.Password;
                    else
                    {
                        new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Podano złe dotychczasowe hasło!") { Owner = this }.ShowDialog();
                        return;
                    }
                }
            }

            if (SQL.SetInstance(Global.Module.USERS, D.User, StswExpress.Globals.Commands.Type.EDIT))
			    Close();
		}
	}
}
