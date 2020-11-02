using System.ComponentModel;
using System.Data;
using System.Reflection;
using WBZ.Models;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsStatus : INotifyPropertyChanged
    {
		/// Family
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
		/// Main family contact
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
		/// Window title
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
