﻿using System.Collections.Generic;

namespace WBZ.Models
{
	public class M_User : M
	{
		/// <summary>
		/// Name
		/// </summary>
		public override string Name => $"{Lastname} {Forename}";
		public string Forename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;

		/// <summary>
		/// Username
		/// </summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>
		/// Newpass
		/// </summary>
		public string Newpass { get; set; } = string.Empty;

		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; set; } = string.Empty;
		
		/// <summary>
		/// Phone
		/// </summary>
		public string Phone { get; set; } = string.Empty;

		/// <summary>
		/// Blocked
		/// </summary>
		public bool Blocked { get; set; } = false;

		/// <summary>
		/// Perms
		/// </summary>
		public List<string> Perms { get; set; } = new List<string>();
	}
}
