using System;
using System.Windows;
using WBZ.Globals;
using WBZ.Modules._base;

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
		private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tbOldPassword.Password) && !string.IsNullOrEmpty(tbNewPassword.Password) && !string.IsNullOrEmpty(tbRNewPassword.Password))
                {
                    if (tbNewPassword.Password != tbRNewPassword.Password)
                    {
                        new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Nowe hasło nie zgadza się z powtórzeniem!") { Owner = this }.ShowDialog();
                        return;
                    }
                    else
                    {
                        if (SQL.Login(Config.User.Login, Global.sha256(tbOldPassword.Password)))
                            D.User.Newpass = tbNewPassword.Password;
                        else
                        {
                            new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Podano złe dotychczasowe hasło!") { Owner = this }.ShowDialog();
                            return;
                        }
                    }
                }

                if (SQL.SetInstance(Config.GetModule(nameof(Users)), D.User, StswExpress.Commands.Type.EDIT))
                    Close();
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd zapisywania zmian w profilu", ex, Config.ListModules[0]);
            }
        }
    }
}
