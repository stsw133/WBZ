using StswExpress;
using StswExpress.Translate;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        readonly D_Settings D = new D_Settings();

        public Settings()
        {
            InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// EmailPassword - Loaded
        /// </summary>
        private void PwdBoxEmailPassword_Loaded(object sender, RoutedEventArgs e)
        {
            if (StswExpress.Settings.Default.mail_Password.Length > 0)
                (sender as PasswordBox).Password = Security.Decrypt(StswExpress.Settings.Default.mail_Password);
        }

        /// <summary>
        /// EmailPassword - PasswordChanged
        /// </summary>
        private void PwdBoxEmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password.Length > 0)
                StswExpress.Settings.Default.mail_Password = Security.Encrypt((sender as PasswordBox).Password);
        }

        /// <summary>
        /// EmailTest
        /// </summary>
        private void BtnEmailTest_Click(object sender, RoutedEventArgs e)
        {
            if (Mail.SendMail(StswExpress.Settings.Default.mail_Username, Config.User.Email, string.Empty, string.Empty))
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.INFO, "Test poczty e-mail powiódł się.") { Owner = this }.ShowDialog();
            else
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, "Test poczty e-mail nie powiódł się!") { Owner = this }.ShowDialog();
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TM.Instance.CurrentLanguage = Properties.Settings.Default.Language;

            Properties.Settings.Default.Save();
            StswExpress.Settings.Default.Save();

            Close();
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            Close();
        }
    }
}
