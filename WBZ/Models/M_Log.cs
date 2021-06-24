using System;

namespace WBZ.Models
{
	/// <summary>
	/// Model for Logs
	/// </summary>
	public class M_Log : M, IMM
	{
		public enum LogType
		{
			Log = 1,
			Error = 2
		}

		/// IMM
		public MV Module { get; set; }
		public int Instance { get; set; }

		/// <summary>
		/// User
		/// </summary>
		public int User { get; set; }
		public string UserName { get; set; }

		/// <summary>
		/// Type
		/// </summary>
		public short Type { get; set; } = (short)LogType.Log;

		/// <summary>
		/// Content
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// DateTime
		/// </summary>
		public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
