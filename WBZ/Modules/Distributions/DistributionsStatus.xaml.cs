using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Modules.Distributions
{
	/// <summary>
	/// Interaction logic for DistributionsStatus.xaml
	/// </summary>
	public partial class DistributionsStatus : Window
	{
		M_DistributionsStatus M = new M_DistributionsStatus();

		public DistributionsStatus(C_DistributionFamily family)
		{
			InitializeComponent();
			DataContext = M;

			M.FamilyInfo = SQL.GetFamily(family.Family);
			M.FamilyContactsInfo = SQL.ListContacts("families", family.Family, @"""default""=true");

			if (family.Status == 0) rbStatus0.IsChecked = true;
			if (family.Status == 1) rbStatus1.IsChecked = true;
			if (family.Status == 2) rbStatus2.IsChecked = true;
		}

		private void btnSendSMS_Click(object sender, MouseButtonEventArgs e)
		{
			if (GSM.SendSMS(new string[] { M.FamilyContactsInfo.Rows[0]["phone"].ToString() }))
				(sender as Button).IsEnabled = false;
		}

		private void btnSendEmail_Click(object sender, MouseButtonEventArgs e)
		{
			if (Mail.SendMail(Properties.Settings.Default.config_Email_Email, new string[] { M.FamilyContactsInfo.Rows[0]["email"].ToString() }, "Darowizna do odebrania",
					Properties.Settings.Default.config_GSM_message + $"\n\n\nWiadomosc generowana automatycznie. Proszę nie odpisywać."))
				(sender as Button).IsEnabled = false;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	public class M_DistributionsStatus : INotifyPropertyChanged
	{
		/// Rodzina
		private C_Family familyInfo;
		public C_Family FamilyInfo
		{
			get
			{
				return familyInfo;
			}
			set
			{
				familyInfo = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Główny kontakt rodziny
		private DataTable familyContactsInfo;
		public DataTable FamilyContactsInfo
		{
			get
			{
				return familyContactsInfo;
			}
			set
			{
				familyContactsInfo = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Tytuł okna
		public string Title
		{
			get
			{
				return $"Zmiana statusu rodziny: {FamilyInfo.Lastname}";
			}
		}

		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
