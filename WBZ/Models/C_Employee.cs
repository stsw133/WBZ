namespace WBZ.Models
{
	public class C_Employee
	{
		public int ID { get; set; }
		public int User { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Forename { get; set; }
		public string Lastname { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public string Postcode { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }
		public string UserName { get; set; }

		public C_Employee()
		{
			ID = 0;
			User = 0;
			Email = "";
			Phone = "";
			Forename = "";
			Lastname = "";
			Department = "";
			Position = "";
			Postcode = "";
			City = "";
			Address = "";
			Archival = false;
			Comment = "";
			Icon = null;
			UserName = "";
		}

		public string Fullname
		{
			get
			{
				return $"{Forename} {Lastname}";
			}
		}
	}
}
