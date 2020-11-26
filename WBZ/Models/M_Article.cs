using System.Data;

namespace WBZ.Models
{
	public class M_Article : M
	{
		public string Codename { get; set; }
		public string Name { get; set; }
		public string EAN { get; set; }
		public decimal Price { get; set; }

		public DataTable Measures { get; set; }
		public string Measure { get; set; }
		public decimal AmountRaw { get; set; }
		public decimal Amount { get; set; }
		public decimal ReservedRaw { get; set; }
		public decimal Reserved { get; set; }

		public M_Article()
		{
			Codename = string.Empty;
			Name = string.Empty;
			EAN = string.Empty;

			Measures = new DataTable();
			Measure = string.Empty;
		}
	}
	/*
	public class M_ArticleMeasure
	{
		public int ID { get; set; }
		public int Article { get; set; }
		public string Name { get; set; }
		public double Converter { get; set; }
		public bool Default { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public decimal Reserved { get; set; }

		public M_ArticleMeasure()
		{
			Name = string.Empty;
			Converter = 1;
		}
	}
	*/
}
