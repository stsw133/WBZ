using System;
using System.Collections.Generic;

namespace WBZ.Models
{
	/// <summary>
	/// Static model for Logs sources
	/// </summary>
	public static class MS_Logs
	{
		public static List<MV> Types { get; } = new List<MV>()
		{
			new MV() { Value = 1, Display = "Log" },
			new MV() { Value = 2, Display = "Error" }
		};
	}

	/// <summary>
	/// Model for Logs
	/// </summary>
	public class M_Log : M, IMM
	{
		/// IMM
		public MV Module { get; set; }
		public int InstanceID { get; set; }

		/// <summary>
		/// User
		/// </summary>
		public int UserID { get; set; }
		public string UserName { get; set; }

		/// <summary>
		/// Type
		/// </summary>
		public short Type { get; set; } = (short)MS_Logs.Types[0].Value;

		/// <summary>
		/// Content
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
