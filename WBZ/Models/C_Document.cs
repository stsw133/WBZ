using System;
using System.Data;
using WBZ.Helpers;

namespace WBZ.Models
{
	public class C_Document
	{
		public enum DocumentStatus
		{
			Withdrawn = -1,
			Buffer = 0,
			Approved = 1
		}

		public int ID { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public int Store { get; set; }
		public int Company { get; set; }
		public DateTime fDateIssue { get; set; }
		public DateTime DateIssue { get; set; }
		public short Status { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }
		public DataTable Positions { get; set; }
		public string StoreName { get; set; }
		public string CompanyName { get; set; }
		public int PositionsCount { get; set; }
		public decimal Weight { get; set; }
		public decimal Cost { get; set; }

		public C_Document()
		{
			var stores = SQL.ListInstances<C_Store>(Global.Module.STORES, "true");

			ID = 0;
			Type = "FS";
			Name = "";
			Store = stores.Count == 1 ? stores[0].ID : 0;
			Company = 0;
			fDateIssue = new DateTime(DateTime.Now.Year, 1, 1);
			DateIssue = DateTime.Now;
			Status = (short)DocumentStatus.Buffer;
			Archival = false;
			Comment = "";
			Icon = null;
			Positions = new DataTable();
			StoreName = stores.Count == 1 ? stores[0].Name : "";
			CompanyName = "";
			PositionsCount = 0;
			Weight = 0;
			Cost = 0;
		}
	}

	public class C_DocumentPosition
	{
		public int ID { get; set; }
		public int Document { get; set; }
		public short Position { get; set; }
		public int Article { get; set; }
		public decimal Amount { get; set; }
		public decimal Cost { get; set; }
		public decimal Tax { get; set; }

		public C_DocumentPosition()
		{
			ID = 0;
			Document = 0;
			Position = 0;
			Article = 0;
			Amount = 0;
			Cost = 0;
			Tax = 0;
		}
	}
}
