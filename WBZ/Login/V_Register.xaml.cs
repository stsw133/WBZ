﻿using System;
using System.Windows;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
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

                if (SQL.Register(TxtBoxEmail.Text, TxtBoxLogin.Text, PwdBoxPassword.Password, true))
                    DialogResult = true;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd rejestracji użytkownika", ex, Config.ListModules[0]);
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
