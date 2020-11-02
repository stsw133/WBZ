namespace WBZ.Models
{
	public class C_Address
	{
		public string Address { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string Postcode { get; set; }

		public C_Address()
		{
			Address = "";
			City = "";
			Country = "";
			Postcode = "";
		}
	}
}
