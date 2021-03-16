using System;
using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Log
	{
		public enum LogType
		{
			Log = 1,
			Error = 2
		}

		public int ID { get; set; } = 0;
		public int User { get; set; } = 0;
		public string UserFullname { get; set; } = string.Empty;
		public string Module { get; set; } = string.Empty;
		public int Instance { get; set; } = 0;
		public short Type { get; set; } = (short)LogType.Log;
		public string Content { get; set; } = string.Empty;
		public DateTime fDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime DateTime { get; set; } = DateTime.Now;

		public string TranslatedModule { get => Global.TranslateModule(Module); }
	}
}
