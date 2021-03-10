using StswExpress.Globals;
using System.Windows;
using System.Windows.Media;
using WBZ.Controls;

namespace WBZ.Modules
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		D_Settings D = new D_Settings();

		public Settings()
		{
			InitializeComponent();
			DataContext = D;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var color = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.ThemeColor);
				D.R = color.R;
				D.G = color.G;
				D.B = color.B;
				D.RGB = Properties.Settings.Default.ThemeColor;
			}
			catch { }
		}

		/// <summary>
		/// EmailPassword - Loaded
		/// </summary>
		private void pbEmailPassword_Loaded(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.config_Email_Password.Length > 0)
				pbEmailPassword.Password = Security.Decrypt(Properties.Settings.Default.config_Email_Password);
		}

		/// <summary>
		/// EmailPassword - PasswordChanged
		/// </summary>
		private void pbEmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (pbEmailPassword.Password.Length > 0)
				Properties.Settings.Default.config_Email_Password = Security.Encrypt(pbEmailPassword.Password);
		}

		/// <summary>
		/// EmailTest
		/// </summary>
		private void btnEmailTest_Click(object sender, RoutedEventArgs e)
		{
			if (Mail.SendMail(Mail.Email, new string[] { Globals.Global.User.Email }, string.Empty, string.Empty))
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Test poczty e-mail powiódł się.") { Owner = this }.ShowDialog();
			else
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Test poczty e-mail nie powiódł się!") { Owner = this }.ShowDialog();
		}

		/// <summary>
		/// Accept
		/// </summary>
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
			StswExpress.Globals.Properties.iSize = Properties.Settings.Default.iSize;
			StswExpress.Globals.Properties.Language = Properties.Settings.Default.Language;
			StswExpress.Globals.Properties.ThemeColor = Properties.Settings.Default.ThemeColor = D.RGB;
			Mail.Host = Properties.Settings.Default.config_Email_Host;
			Mail.Port = Properties.Settings.Default.config_Email_Port;
			Mail.Email = Properties.Settings.Default.config_Email_Email;
			Mail.Password = Properties.Settings.Default.config_Email_Password;

			Properties.Settings.Default.Save();
			Close();
		}

		/// <summary>
		/// Cancel
		/// </summary>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.Reload();
			Close();
		}
	}
}
