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

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// DateReal
		/// </summary>
		public DateTime fDateReal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime DateReal { get; set; } = DateTime.Now;

		/// <summary>
		/// Status
		/// </summary>
		public short Status { get; set; } = (short)DistributionStatus.Buffer;

		/// <summary>
		/// Families
		/// </summary>
		public List<M_DistributionFamily> Families { get; set; } = new List<M_DistributionFamily>();

		/// <summary>
		/// FamiliesCount
		/// </summary>
		public int FamiliesCount { get; set; } = 0;

		/// <summary>
		/// MembersCount
		/// </summary>
		public int MembersCount { get; set; } = 0;

		/// <summary>
		/// PositionsCount
		/// </summary>
		public int PositionsCount { get; set; } = 0;

		/// <summary>
		/// Weight
		/// </summary>
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

		/// <summary>
		/// Family
		/// </summary>
		public int Family { get; set; } = 0;

		/// <summary>
		/// FamilyName
		/// </summary>
		public string FamilyName { get; set; } = string.Empty;

		/// <summary>
		/// Members
		/// </summary>
		public short Members { get; set; } = 0;

		/// <summary>
		/// Status
		/// </summary>
		public short Status { get; set; } = (short)DistributionFamilyStatus.None;

		/// <summary>
		/// Positions
		/// </summary>
		public DataTable Positions { get; set; } = SQL.GetDistributionPositionsFormatting();
	}

	public class M_DistributionPosition
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Distribution
		/// </summary>
		public int Distribution { get; set; }
		
		/// <summary>
		/// Position
		/// </summary>
		public short Position { get; set; }

		/// <summary>
		/// Store
		/// </summary>
		public int Store { get; set; }
		public string StoreName { get; set; }

		/// <summary>
		/// Article
		/// </summary>
		public int Article { get; set; }
		public string ArticleName { get; set; } = string.Empty;

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; }

		public M_DistributionPosition()
		{
			var stores = SQL.ListInstances<M_Store>(Global.Module.STORES, "true", null, 1);

			Store = stores.Count == 1 ? stores[0].ID : 0;
			StoreName = stores.Count == 1 ? stores[0].Name : string.Empty;
		}
	}
}
