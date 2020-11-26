using System;
using System.Collections.Generic;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Distribution : M
	{
		public enum DistributionStatus
		{
			Withdrawn = -1,
			Buffer = 0,
			Approved = 1
		}

		public string Name { get; set; }
		public DateTime fDateReal { get; set; }
		public DateTime DateReal { get; set; }
		public short Status { get; set; }
		public List<M_DistributionFamily> Families { get; set; }
		public int FamiliesCount { get; set; }
		public int MembersCount { get; set; }
		public int PositionsCount { get; set; }
		public decimal Weight { get; set; }

		public M_Distribution()
		{
			Name = string.Empty;
			fDateReal = new DateTime(DateTime.Now.Year, 1, 1);
			DateReal = DateTime.Now;
			Status = (short)DistributionStatus.Buffer;
			Families = new List<M_DistributionFamily>();
		}
	}

	public class M_DistributionFamily
	{
		public enum DistributionFamilyStatus
		{
			None = 0,
			Informed = 1,
			Taken = 2
		}

		public int Family { get; set; }
		public string FamilyName { get; set; }
		public short Members { get; set; }
		public short Status { get; set; }
		public DataTable Positions { get; set; }

		public M_DistributionFamily()
		{
			FamilyName = string.Empty;
			Status = (short)DistributionFamilyStatus.None;
			Positions = SQL.GetDistributionPositionsFormatting();
		}
	}

	public class M_DistributionPosition
	{
		public int ID { get; set; }
		public int Distribution { get; set; }
		public short Position { get; set; }
		public int Store { get; set; }
		public int Article { get; set; }
		public decimal Amount { get; set; }
		public string StoreName { get; set; }
		public string ArticleName { get; set; }

		public M_DistributionPosition()
		{
			var stores = SQL.ListInstances<M_Store>(Global.Module.STORES, "true");

			Store = stores.Count == 1 ? stores[0].ID : 0;
			StoreName = stores.Count == 1 ? stores[0].Name : string.Empty;
			ArticleName = string.Empty;
		}
	}
}
