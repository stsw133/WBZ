using System;
using System.Collections.Generic;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
	/// <summary>
	/// Model for Distributions
	/// </summary>
	public class M_Distribution : M
	{
		public enum DistributionStatus
		{
			Withdrawn = -1,
			Buffer = 0,
			Approved = 1
		}

		/// <summary>
		/// DateReal
		/// </summary>
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
		public int FamiliesCount { get; set; }

		/// <summary>
		/// MembersCount
		/// </summary>
		public int MembersCount { get; set; }

		/// <summary>
		/// PositionsCount
		/// </summary>
		public int PositionsCount { get; set; }

		/// <summary>
		/// Weight
		/// </summary>
		public decimal Weight { get; set; }
	}

	/// <summary>
	/// Model for DistributionsFamilies
	/// </summary>
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
		public int FamilyID { get; set; }
		public string FamilyName { get; set; }

		/// <summary>
		/// Members
		/// </summary>
		public short? Members { get; set; }

		/// <summary>
		/// Status
		/// </summary>
		public short Status { get; set; } = (short)DistributionFamilyStatus.None;

		/// <summary>
		/// Positions
		/// </summary>
		public DataTable Positions { get; set; } = SQL.GetDistributionPositionsFormatting();
	}

	/// <summary>
	/// Model for DistributionsPositions
	/// </summary>
	public class M_DistributionPosition
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Distribution
		/// </summary>
		public int DistributionID { get; set; }
		
		/// <summary>
		/// Pos
		/// </summary>
		public short Pos { get; set; }

		/// <summary>
		/// Store
		/// </summary>
		public int StoreID { get; set; }
		public string StoreName { get; set; }

		/// <summary>
		/// Article
		/// </summary>
		public int ArticleID { get; set; }
		public string ArticleName { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		public decimal Quantity { get; set; }

		public M_DistributionPosition()
		{
			var stores = SQL.ComboSource(Config.GetModule(nameof(Modules.Stores)), "codename", "true", false);
			if (stores.Count > 0 && StoreID == 0)
			{
				StoreID = (int)stores[0].Value;
				StoreName = (string)stores[0].Display;
			}
		}
	}
}
