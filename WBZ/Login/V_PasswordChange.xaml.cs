using StswExpress;
using System;
using System.Windows;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for PasswordChange.xaml
    /// </summary>
    public partial class PasswordChange : Window
    {
        readonly D_PasswordChange D = new D_PasswordChange();

        public PasswordChange(string email)
        {
            InitializeComponent();
            DataContext = D;

            D.Email = email;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(PwdBoxPassword.Password))
                {
                    new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Nie podano nowego hasła!") { Owner = this }.ShowDialog();
                    return;
                }
                else if (PwdBoxPassword.Password != PwdBoxRepass.Password)
                {
                    new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Nowe hasło nie zgadza się z powtórzeniem!") { Owner = this }.ShowDialog();
                    return;
                };

                var user = SQL.ListInstances<M_User>(Config.GetModule(nameof(Modules.Users)), $"use.email='{D.Email}'")?[0];
                user.Newpass = PwdBoxPassword.Password;
                if (SQL.SetInstance(Config.GetModule(nameof(Modules.Users)), user, Commands.Type.EDIT))
                    DialogResult = true;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd podczas zmiany hasła", ex, Config.ListModules[0]);
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
