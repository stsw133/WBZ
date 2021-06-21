using System;
using System.Data;

namespace WBZ.Models
{
	/// <summary>
	/// Model for Documents
	/// </summary>
	public class M_Document : M
	{
		public enum DocumentStatus
		{
			Withdrawn = -1,
			Buffer = 0,
			Approved = 1
		}

		/// <summary>
		/// Type
		/// </summary>
		public string Type { get; set; } = "FS";

		/// <summary>
		/// Store
		/// </summary>
		public int Store { get; set; }
		public string StoreName { get; set; }

		/// <summary>
		/// Contractor
		/// </summary>
		public int Contractor { get; set; }
		public string ContractorName { get; set; }

		/// <summary>
		/// DateIssue
		/// </summary>
		public DateTime DateIssue { get; set; } = DateTime.Now;

		/// <summary>
		/// Status
		/// </summary>
		public short Status { get; set; } = (short)DocumentStatus.Buffer;

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
		/// Position
		/// </summary>
		public short Position { get; set; }

		/// <summary>
		/// Article
		/// </summary>
		public int Article { get; set; }

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; }

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
