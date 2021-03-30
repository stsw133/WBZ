using System;

namespace WBZ.Models
{
	public class M_Log
	{
		public enum LogType
		{
			Log = 1,
			Error = 2
		}

		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; } = 0;

		/// <summary>
		/// User
		/// </summary>
		public int User { get; set; } = 0;
		public string UserFullname { get; set; } = string.Empty;

		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => TranslateMe.TM.Tr(char.ToUpper(Module[0]) + Module[1..], languageId: StswExpress.Globals.Properties.Language);

		/// <summary>
		/// Instance
		/// </summary>
		public int Instance { get; set; } = 0;

		/// <summary>
		/// Type
		/// </summary>
		public short Type { get; set; } = (short)LogType.Log;

		/// <summary>
		/// Content
		/// </summary>
		public string Content { get; set; } = string.Empty;

		/// <summary>
		/// DateTime
		/// </summary>
		public DateTime fDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime DateTime { get; set; } = DateTime.Now;
	}
}
