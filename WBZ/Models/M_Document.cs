using StswExpress.Translate;
using System;
using System.Collections.Generic;
using System.Data;

namespace WBZ.Models
{
	/// <summary>
	/// Static model for Documents sources
	/// </summary>
	public static class MS_Documents
	{
		public static List<MV> Statuses { get; } = new List<MV>()
		{
			new MV() { Value = 0, Display = TM.Tr("ToBuffer") },
			new MV() { Value = 1, Display = TM.Tr("Approved") },
			new MV() { Value = 2, Display = TM.Tr("InProgress") },
			new MV() { Value = 3, Display = TM.Tr("Closed") }
		};

		public static List<MV> Types { get; } = new List<MV>()
		{
			new MV() { Value = "fs", Display = "Faktura sprzedaży" }
		};
	}

	/// <summary>
	/// Model for Documents
	/// </summary>
	public class M_Document : M
	{
		/// <summary>
		/// Type
		/// </summary>
		public string Type { get; set; } = (string)MS_Documents.Types[0].Value;

		/// <summary>
		/// Store
		/// </summary>
		public int StoreID { get; set; }
		public string StoreName { get; set; }

		/// <summary>
		/// Contractor
		/// </summary>
		public int ContractorID { get; set; }
		public string ContractorName { get; set; }

		/// <summary>
		/// DateIssue
		/// </summary>
		public DateTime DateIssue { get; set; } = DateTime.Now;

		/// <summary>
		/// Status
		/// </summary>
		public short Status { get; set; } = (short)MS_Documents.Types[0].Value;

		/// <summary>
		/// Positions
		/// </summary>
		public DataTable Positions { get; set; } = new DataTable();
		
		/// <summary>
		/// PositionsCount
		/// </summary>
		public int PositionsCount { get; set; }

		/// <summary>
		/// Weight
		/// </summary>
		public decimal Weight { get; set; }

		/// <summary>
		/// Net
		/// </summary>
		public decimal Net { get; set; }

		/// <summary>
		/// Tax
		/// </summary>
		public decimal Tax { get; set; }
	}

	/// <summary>
	/// Model for DocumentsPositions
	/// </summary>
	public class M_DocumentPosition
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; }
		
		/// <summary>
		/// Pos
		/// </summary>
		public short Pos { get; set; }

		/// <summary>
		/// Article
		/// </summary>
		public int ArticleID { get; set; }
		public int ArticleName { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		/// Net
		/// </summary>
		public decimal Net { get; set; }

		/// <summary>
		/// Tax
		/// </summary>
		public decimal Tax { get; set; }
	}
}
