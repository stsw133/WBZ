using System;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
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
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Store
		/// </summary>
		public MV cStore { get; set; } = new MV();
		public int Store
		{
			get => cStore.ID;
			set => cStore = SQL.ListValues(Config.Modules.STORES, "codename", $"id={value}", false)?[0];
		}

		/// <summary>
		/// Contractor
		/// </summary>
		public int Contractor { get; set; } = 0;
		public string ContractorName { get; set; } = string.Empty;

		/// <summary>
		/// DateIssue
		/// </summary>
		public DateTime fDateIssue { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
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
		public int PositionsCount { get; set; } = 0;

		/// <summary>
		/// Weight
		/// </summary>
		public decimal Weight { get; set; } = 0;

		/// <summary>
		/// Cost
		/// </summary>
		public decimal Cost { get; set; } = 0;
	}

	public class M_DocumentPosition
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; } = 0;
		
		/// <summary>
		/// Position
		/// </summary>
		public short Position { get; set; } = 0;

		/// <summary>
		/// Article
		/// </summary>
		public int Article { get; set; } = 0;

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; } = 0;

		/// <summary>
		/// Cost
		/// </summary>
		public decimal Cost { get; set; } = 0;

		/// <summary>
		/// Tax
		/// </summary>
		public decimal Tax { get; set; } = 0;
	}
}
