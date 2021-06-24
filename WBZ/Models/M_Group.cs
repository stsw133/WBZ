﻿namespace WBZ.Models
{
	/// <summary>
	/// Model for Groups
	/// </summary>
	public class M_Group : M, IMM
	{
		/// IMM
		public MV Module { get; set; }
		public int Instance { get; set; }

		/// <summary>
		/// Owner
		/// </summary>
		public int Owner { get; set; }

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; set; }
		public string Fullpath => Path + Name;
    }
}
