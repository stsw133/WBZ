namespace WBZ.Models
{
	/// <summary>
	/// Model for Stores
	/// </summary>
	public class M_Store : M, IMA
	{
		/// IMA
		public string Address { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string Postcode { get; set; }

		/// <summary>
		/// Codename
		/// </summary>
		public string Codename { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		/// Reserved
		/// </summary>
		public decimal Reserved { get; set; }
    }
}
