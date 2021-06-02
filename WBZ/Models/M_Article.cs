using System.Data;

namespace WBZ.Models
{
	public class M_Article : M
	{
		/// <summary>
		/// Codename
		/// </summary>
		public string Codename { get; set; } = string.Empty;

		/// <summary>
		/// Name
		/// </summary>
		public override string Name { get; set; } = string.Empty;

		/// <summary>
		/// EAN
		/// </summary>
		public string EAN { get; set; } = string.Empty;

		/// <summary>
		/// Price
		/// </summary>
		public decimal Price { get; set; } = 0;

		/// <summary>
		/// MainStore
		/// </summary>
		public M_Store MainStore { get; set; } = new M_Store();

		/// <summary>
		/// Measures
		/// </summary>
		public DataTable Measures { get; set; } = new DataTable();

		/// <summary>
		/// Measure
		/// </summary>
		public string Measure { get; set; } = string.Empty;

		/// <summary>
		/// AmountRaw
		/// </summary>
		public decimal AmountRaw { get; set; } = 0;

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; } = 0;

		/// <summary>
		/// ReservedRaw
		/// </summary>
		public decimal ReservedRaw { get; set; } = 0;

		/// <summary>
		/// Reserved
		/// </summary>
		public decimal Reserved { get; set; } = 0;
	}

	public class M_ArticleMeasure
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; } = 0;

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Converter
		/// </summary>
		public double Converter { get; set; } = 1;

		/// <summary>
		/// Default
		/// </summary>
		public bool Default { get; set; } = false;

		/// <summary>
		/// Price
		/// </summary>
		public decimal Price { get; set; } = 0;

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; } = 0;

		/// <summary>
		/// Reserved
		/// </summary>
		public decimal Reserved { get; set; } = 0;
	}
}
