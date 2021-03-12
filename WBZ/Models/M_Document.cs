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

		public string Type { get; set; } = "FS";
		public string Name { get; set; } = string.Empty;
		public int Store { get; set; }
		public int Contractor { get; set; } = 0;
		public DateTime fDateIssue { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime DateIssue { get; set; } = DateTime.Now;
		public short Status { get; set; } = (short)DocumentStatus.Buffer;
		public DataTable Positions { get; set; } = new DataTable();
		public string StoreName { get; set; }
		public string ContractorName { get; set; } = string.Empty;
		public int PositionsCount { get; set; } = 0;
		public decimal Weight { get; set; } = 0;
		public decimal Cost { get; set; } = 0;

		public M_Document()
		{
			var stores = SQL.ListInstances<M_Store>(Global.Module.STORES, "true");

			Store = stores.Count == 1 ? stores[0].ID : 0;
			StoreName = stores.Count == 1 ? stores[0].Name : string.Empty;
		}
	}

	public class M_DocumentPosition
	{
		public int ID { get; set; } = 0;
		public int Document { get; set; } = 0;
		public short Position { get; set; } = 0;
		public int Article { get; set; } = 0;
		public decimal Amount { get; set; } = 0;
		public decimal Cost { get; set; } = 0;
		public decimal Tax { get; set; } = 0;
	}
}
