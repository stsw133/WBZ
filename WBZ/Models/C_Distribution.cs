using System;
using System.Collections.Generic;
using System.Data;
using WBZ.Helpers;

namespace WBZ.Models
{
	public class C_Distribution
	{
		public enum DistributionStatus
		{
			Withdrawn = -1,
			Buffer = 0,
			Approved = 1
		}

		public int ID { get; set; }
		public string Name { get; set; }
		public DateTime fDateReal { get; set; }
		public DateTime DateReal { get; set; }
		public short Status { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }
		public List<C_DistributionFamily> Families { get; set; }
		public int FamiliesCount { get; set; }
		public int MembersCount { get; set; }
		public int PositionsCount { get; set; }
		public decimal Weight { get; set; }

		public C_Distribution()
		{
			ID = 0;
			Name = "";
			fDateReal = new DateTime(DateTime.Now.Year, 1, 1);
			DateReal = DateTime.Now;
			Status = (short)DistributionStatus.Buffer;
			Archival = false;
			Comment = "";
			Icon = null;
			Families = new List<C_DistributionFamily>();
			FamiliesCount = 0;
			MembersCount = 0;
			PositionsCount = 0;
			Weight = 0;
		}
	}

	public class C_DistributionFamily
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

		public C_DistributionFamily()
		{
			Family = 0;
			FamilyName = "";
			Members = 0;
			Status = (short)DistributionFamilyStatus.None;
			Positions = SQL.GetDistributionPositionsFormatting();
		}
	}

	public class C_DistributionPosition
	{
		public int ID { get; set; }
		public int Distribution { get; set; }
		public short Position { get; set; }
		public int Store { get; set; }
		public int Article { get; set; }
		public decimal Amount { get; set; }
		public string StoreName { get; set; }
		public string ArticleName { get; set; }

		public C_DistributionPosition()
		{
			var stores = SQL.ListInstances(Global.Module.STORES, "true").DataTableToList<C_Store>();

			ID = 0;
			Distribution = 0;
			Position = 0;
			Store = stores.Count == 1 ? stores[0].ID : 0;
			Article = 0;
			Amount = 0;
			StoreName = stores.Count == 1 ? stores[0].Name : "";
			ArticleName = "";
		}
	}
}
