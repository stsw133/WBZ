using System.ComponentModel;
using System.Data;
using System.Reflection;
using WBZ.Models;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsStatus : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Window title
		public string Title
		{
			get
			{
				return $"Zmiana statusu rodziny: {FamilyInfo.Lastname}";
			}
		}
		/// Family
		private M_Family familyInfo;
		public M_Family FamilyInfo
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
	}
}
