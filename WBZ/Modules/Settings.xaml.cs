using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Helpers;

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

		private void pbEmailPassword_Loaded(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.config_Email_Password.Length > 0)
				pbEmailPassword.Password = Global.Decrypt(Properties.Settings.Default.config_Email_Password);
		}
		private void pbEmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (pbEmailPassword.Password.Length > 0)
				Properties.Settings.Default.config_Email_Password = Global.Encrypt(pbEmailPassword.Password);
		}

		/// <summary>
		/// Test
		/// </summary>
		private void btnEmailTest_Click(object sender, MouseButtonEventArgs e)
		{
			if (Mail.TestMail())
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Test poczty e-mail powiódł się.") { Owner = this }.ShowDialog();
			else
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Test poczty e-mail nie powiódł się!") { Owner = this }.ShowDialog();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}
}
