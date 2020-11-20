namespace WBZ.Models
{
	public class C_Company
	{
		public int ID { get; set; }
		public string Codename { get; set; }
		public string Name { get; set; }
		public string Branch { get; set; }
		public string NIP { get; set; }
		public string REGON { get; set; }
		public string Postcode { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }

		public C_Company()
		{
			ID = 0;
			Codename = "";
			Name = "";
			Branch = "";
			NIP = "";
			REGON = "";
			Postcode = "";
			City = "";
			Address = "";
			Archival = false;
			Comment = "";
			Icon = null;
		}
	}
}
