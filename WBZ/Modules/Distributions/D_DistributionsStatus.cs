using StswExpress;
using System.Data;
using WBZ.Models;

namespace WBZ.Modules.Distributions
{
    class D_DistributionsStatus : D
    {
		/// Window title
		public string Title => $"Zmiana statusu rodziny: {FamilyInfo?.Lastname}";

		/// Family
		private M_Family familyInfo;
		public M_Family FamilyInfo
		{
			get => familyInfo;
			set => SetField(ref familyInfo, value, () => FamilyInfo);
		}

		/// Main family contact
		private DataTable familyContactsInfo;
		public DataTable FamilyContactsInfo
		{
			get => familyContactsInfo;
			set => SetField(ref familyContactsInfo, value, () => FamilyContactsInfo);
		}
	}
}
