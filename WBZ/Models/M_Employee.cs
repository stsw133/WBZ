namespace WBZ.Models
{
	public class M_Employee : M_Address
	{
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Forename { get; set; }
		public string Lastname { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }

		public M_Employee()
        {
			Email = string.Empty;
			Phone = string.Empty;
			Forename = string.Empty;
			Lastname = string.Empty;
			Department = string.Empty;
			Position = string.Empty;
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
