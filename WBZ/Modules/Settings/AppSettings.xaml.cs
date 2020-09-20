using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Helpers;

namespace WBZ.Modules.Settings
{
	/// <summary>
	/// Interaction logic for AppSettings.xaml
	/// </summary>
	public partial class AppSettings : Window
	{
		public AppSettings()
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

		private void btnEmailTest_Click(object sender, MouseButtonEventArgs e)
		{
			if (Mail.TestMail())
				MessageBox.Show("Test poczty e-mail powiódł się.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
			else
				MessageBox.Show("Test poczty e-mail nie powiódł się!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}
}
