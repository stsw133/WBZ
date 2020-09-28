using System;
using System.Data;

namespace WBZ.Classes
{
	public class C_Family
	{
		public int ID { get; set; }
		public string Declarant { get; set; }
		public string Lastname { get; set; }
		public short Members { get; set; }
		public string Postcode { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public bool Status { get; set; }
		public bool C_SMS { get; set; }
		public bool C_Call { get; set; }
		public bool C_Email { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public DateTime DonationLast { get; set; }
		public decimal DonationWeight { get; set; }

		public C_Family()
		{
			ID = 0;
			Declarant = "";
			Lastname = "";
			Members = 0;
			Postcode = "";
			City = "";
			Address = "";
			Status = true;
			C_SMS = false;
			C_Call = false;
			C_Email = false;
			Archival = false;
			Comment = "";
			DonationLast = DateTime.MinValue;
			DonationWeight = 0;
		}
	}
}
