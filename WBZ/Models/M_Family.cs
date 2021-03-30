using System;

namespace WBZ.Models
{
	public class M_Family : MA
	{
		/// <summary>
		/// Declarant
		/// </summary>
		public string Declarant { get; set; } = string.Empty;

		/// <summary>
		/// Lastname
		/// </summary>
		public string Lastname { get; set; } = string.Empty;

		/// <summary>
		/// Members
		/// </summary>
		public short Members { get; set; } = 0;

		/// <summary>
		/// Status
		/// </summary>
		public bool Status { get; set; } = false;

		/// <summary>
		/// C_*
		/// </summary>
		public bool C_SMS { get; set; } = false;
		public bool C_Call { get; set; } = false;
		public bool C_Email { get; set; } = false;

		/// <summary>
		/// Donation
		/// </summary>
		public DateTime? DonationLast { get; set; } = null;
		public decimal DonationWeight { get; set; } = 0;
	}
}
