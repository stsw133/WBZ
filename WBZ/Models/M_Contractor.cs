namespace WBZ.Models
{
	/// <summary>
	/// Model for Contractors
	/// </summary>
	public class M_Contractor : M, IMA
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
		/// Branch
		/// </summary>
		public string Branch { get; set; }

		/// <summary>
		/// NIP
		/// </summary>
		public string NIP { get; set; }

		/// <summary>
		/// REGON
		/// </summary>
		public string REGON { get; set; }
    }
}
