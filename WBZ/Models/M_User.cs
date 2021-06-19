using System.Collections.Generic;

namespace WBZ.Models
{
	/// <summary>
	/// Model for Users
	/// </summary>
	public class M_User : M, IMP
	{
		/// IMP
		public override string Name => Fullname;
		public string Forename { get; set; }
		public string Lastname { get; set; }
		public string Fullname => $"{Lastname} {Forename}";
		public string Email { get; set; }
		public string Phone { get; set; }

		/// <summary>
		/// Username
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Newpass
		/// </summary>
		public string Newpass { get; set; }

		/// <summary>
		/// Blocked
		/// </summary>
		public bool Blocked { get; set; }

		/// <summary>
		/// Perms
		/// </summary>
		public List<string> Perms { get; set; } = new List<string>();
    }
}
