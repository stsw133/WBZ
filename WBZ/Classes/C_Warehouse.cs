using System.Collections.Generic;

namespace WBZ.Classes
{
	public class C_Store
	{
		public int ID { get; set; }
		public string Codename { get; set; }
		public string Name { get; set; }
		public string Postcode { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public decimal Amount { get; set; }
		public decimal Reserved { get; set; }

		public C_Store()
		{
			ID = 0;
			Codename = "";
			Name = "";
			Postcode = "";
			City = "";
			Address = "";
			Archival = false;
			Comment = "";
			Amount = 0;
			Reserved = 0;
		}
	}
}
