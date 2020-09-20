using System.Data;

namespace WBZ.Classes
{
	public class C_Article
	{
		public int ID { get; set; }
		public string Codename { get; set; }
		public string Name { get; set; }
		public string EAN { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public DataTable Measures { get; set; }
		public string Measure { get; set; }
		public decimal AmountRaw { get; set; }
		public decimal Amount { get; set; }
		public decimal ReservedRaw { get; set; }
		public decimal Reserved { get; set; }

		public C_Article()
		{
			ID = 0;
			Codename = "";
			Name = "";
			EAN = "";
			Archival = false;
			Comment = "";
			Measures = new DataTable();
			Measure = "";
			AmountRaw = 0;
			Amount = 0;
			ReservedRaw = 0;
			Reserved = 0;
		}
	}

	public class C_ArticleMeasure
	{
		public int ID { get; set; }
		public int Article { get; set; }
		public string Name { get; set; }
		public double Converter { get; set; }
		public bool Default { get; set; }
		public decimal Amount { get; set; }
		public decimal Reserved { get; set; }

		public C_ArticleMeasure()
		{
			ID = 0;
			Article = 0;
			Name = "";
			Converter = 1;
			Default = false;
			Amount = 0;
			Reserved = 0;
		}
	}
}
