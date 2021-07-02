using StswExpress;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Globals;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsStatus.xaml
	/// </summary>
	public partial class DistributionsStatus : Window
	{
		D_DistributionsStatus D = new D_DistributionsStatus();

		public DistributionsStatus(M_DistributionFamily family)
		{
			InitializeComponent();
			DataContext = D;

			D.FamilyInfo = SQL.GetInstance<M_Family>(Config.GetModule(nameof(Modules.Families)), family.FamilyID);
			D.FamilyContactsInfo = SQL.ListContacts(Config.GetModule(nameof(Modules.Families)), family.FamilyID, @"""default""=true");

			if (family.Status == 0) rbStatus0.IsChecked = true;
			if (family.Status == 1) rbStatus1.IsChecked = true;
			if (family.Status == 2) rbStatus2.IsChecked = true;
		}

		/// <summary>
		/// Send SMS
		/// </summary>
		private void btnSendSMS_Click(object sender, RoutedEventArgs e)
		{
			if (GSM.SendSMS(new string[] { D.FamilyContactsInfo.Rows[0]["phone"].ToString() }))
				(sender as Button).IsEnabled = false;
		}

		/// <summary>
		/// Send Email
		/// </summary>
		private void btnSendEmail_Click(object sender, RoutedEventArgs e)
		{
			if (Mail.SendMail(StswExpress.Settings.Default.mail_Username, new string[] { D.FamilyContactsInfo.Rows[0]["email"].ToString() }, "Darowizna do odebrania",
					Properties.Settings.Default.config_GSM_message + $"\n\n\nWiadomosc generowana automatycznie. Proszę nie odpisywać."))
				(sender as Button).IsEnabled = false;
		}

		/// <summary>
		/// OK
		/// </summary>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		/// <summary>
		/// Cancel
		/// </summary>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
