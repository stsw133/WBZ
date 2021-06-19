namespace WBZ.Models
{
	/// <summary>
	/// Model for Employees
	/// </summary>
	public class M_Employee : M, IMA, IMP
	{
		/// IMA
		public string Address { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string Postcode { get; set; }

		/// IMP
		public override string Name => Fullname;
		public string Forename { get; set; }
		public string Lastname { get; set; }
		public string Fullname => $"{Lastname} {Forename}";
		public string Email { get; set; }
		public string Phone { get; set; }

		/// <summary>
		/// Department
		/// </summary>
		public string Department { get; set; }

		/// <summary>
		/// Position
		/// </summary>
		public string Position { get; set; }
    }
}
