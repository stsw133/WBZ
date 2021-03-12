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

		public string Name { get; set; } = string.Empty;
		public DateTime fDateReal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime DateReal { get; set; } = DateTime.Now;
		public short Status { get; set; } = (short)DistributionStatus.Buffer;
		public List<M_DistributionFamily> Families { get; set; } = new List<M_DistributionFamily>();
		public int FamiliesCount { get; set; } = 0;
		public int MembersCount { get; set; } = 0;
		public int PositionsCount { get; set; } = 0;
		public decimal Weight { get; set; } = 0;
	}

	public class M_DistributionFamily
	{
		public enum DistributionFamilyStatus
		{
			None = 0,
			Informed = 1,
			Taken = 2
		}

		public int Family { get; set; } = 0;
		public string FamilyName { get; set; } = string.Empty;
		public short Members { get; set; } = 0;
		public short Status { get; set; } = (short)DistributionFamilyStatus.None;
		public DataTable Positions { get; set; } = SQL.GetDistributionPositionsFormatting();
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
		public string ArticleName { get; set; } = string.Empty;

		public M_DistributionPosition()
		{
			var stores = SQL.ListInstances<M_Store>(Global.Module.STORES, "true");

			Store = stores.Count == 1 ? stores[0].ID : 0;
			StoreName = stores.Count == 1 ? stores[0].Name : string.Empty;
		}
	}
}
