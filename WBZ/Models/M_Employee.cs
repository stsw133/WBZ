namespace WBZ.Models
{
	public class M_Employee : MA
	{
		/// <summary>
		/// Name
		/// </summary>
		public override string Name => $"{Lastname} {Forename}";
		public string Forename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;

		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Phone
		/// </summary>
		public string Phone { get; set; } = string.Empty;

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
