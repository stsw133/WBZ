using System;

namespace WBZ.Models
{
	public class M_Family : MA
	{
		public string Declarant { get; set; }
		public string Lastname { get; set; }
		public short Members { get; set; }
		public bool Status { get; set; }
		public bool C_SMS { get; set; }
		public bool C_Call { get; set; }
		public bool C_Email { get; set; }
		public DateTime? DonationLast { get; set; }
		public decimal DonationWeight { get; set; }

		public M_Family()
        {
			Declarant = string.Empty;
			Lastname = string.Empty;
        }
	}
}
