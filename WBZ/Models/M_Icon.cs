﻿namespace WBZ.Models
{
	/// <summary>
	/// Model for Icons
	/// </summary>
	public class M_Icon : M, IMM
	{
		/// IMM
		public MV Module { get; set; }
		public int Instance { get; set; }

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Format
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		/// Dimensions
		/// </summary>
		public int Height { get; set; }
		public int Width { get; set; }

		/// <summary>
		/// Size
		/// </summary>
		public double Size { get; set; }

		/// <summary>
		/// File
		/// </summary>
		public byte[] File { get; set; }
    }
}
