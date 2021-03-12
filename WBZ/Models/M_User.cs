using System.Collections.Generic;

namespace WBZ.Models
{
	public class M_User : M
	{
		public string Username { get; set; } = string.Empty;
		public string Newpass { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Forename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public bool Blocked { get; set; } = false;
		public List<string> Perms { get; set; } = new List<string>();

		public string Fullname { get => $"{Lastname} {Forename}"; }
	}
}
