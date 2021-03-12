using System;

namespace WBZ.Models
{
	public class M_Family : MA
	{
		public string Declarant { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public short Members { get; set; } = 0;
		public bool Status { get; set; } = false;
		public bool C_SMS { get; set; } = false;
		public bool C_Call { get; set; } = false;
		public bool C_Email { get; set; } = false;
		public DateTime? DonationLast { get; set; } = null;
		public decimal DonationWeight { get; set; } = 0;
	}
}
