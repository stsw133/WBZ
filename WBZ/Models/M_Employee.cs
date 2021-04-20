namespace WBZ.Models
{
	public class M_Employee : MA
	{
		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Phone
		/// </summary>
		public string Phone { get; set; } = string.Empty;

		/// <summary>
		/// Name
		/// </summary>
		public string Forename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string Fullname => $"{Lastname} {Forename}";

		/// <summary>
		/// Department
		/// </summary>
		public string Department { get; set; } = string.Empty;

		/// <summary>
		/// Position
		/// </summary>
		public string Position { get; set; } = string.Empty;
	}
}
