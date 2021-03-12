namespace WBZ.Models
{
	public class M_Employee : MA
	{
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Forename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string Department { get; set; } = string.Empty;
		public string Position { get; set; } = string.Empty;

		public string Fullname { get => $"{Lastname} {Forename}"; }
	}
}
