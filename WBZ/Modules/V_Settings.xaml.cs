using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Globals;

namespace WBZ.Modules
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		public Settings()
		{
			InitializeComponent();
		}

		/// <summary>
		/// EmailPassword - Loaded
		/// </summary>
		private void pbEmailPassword_Loaded(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.config_Email_Password.Length > 0)
				pbEmailPassword.Password = Global.Decrypt(Properties.Settings.Default.config_Email_Password);
		}

		/// <summary>
		/// EmailPassword - PasswordChanged
		/// </summary>
		private void pbEmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (pbEmailPassword.Password.Length > 0)
				Properties.Settings.Default.config_Email_Password = Global.Encrypt(pbEmailPassword.Password);
		}

		/// <summary>
		/// EmailTest
		/// </summary>
		private void btnEmailTest_Click(object sender, MouseButtonEventArgs e)
		{
			if (Mail.TestMail())
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Test poczty e-mail powiódł się.") { Owner = this }.ShowDialog();
			else
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Test poczty e-mail nie powiódł się!") { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Closed
		/// </summary>
		private void Window_Closed(object sender, EventArgs e)
		{
			StswExpress.Globals.Properties.iSize = Properties.Settings.Default.iSize;
			StswExpress.Globals.Properties.Language = Properties.Settings.Default.Language;
			Properties.Settings.Default.Save();
			(Owner as Main).menuRefresh_Executed(null, null);
		}
    }
}
