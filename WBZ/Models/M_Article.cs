using System.Data;

namespace WBZ.Models
{
	public class M_Article : M
	{
		public string Codename { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string EAN { get; set; } = string.Empty;
		public decimal Price { get; set; } = 0;
		public M_Store MainStore { get; set; } = new M_Store();

		public DataTable Measures { get; set; } = new DataTable();
		public string Measure { get; set; } = string.Empty;
		public decimal AmountRaw { get; set; } = 0;
		public decimal Amount { get; set; } = 0;
		public decimal ReservedRaw { get; set; } = 0;
		public decimal Reserved { get; set; } = 0;
	}
	
	public class M_ArticleMeasure
	{
		public int ID { get; set; } = 0;
		public int Article { get; set; } = 0;
		public string Name { get; set; } = string.Empty;
		public double Converter { get; set; } = 1;
		public bool Default { get; set; } = false;
		public decimal Price { get; set; } = 0;
		public decimal Amount { get; set; } = 0;
		public decimal Reserved { get; set; } = 0;
	}
}
