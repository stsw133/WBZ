using System;

namespace WBZ.Classes
{
	public class C_Log
	{
		public int ID { get; set; }
		public int User { get; set; }
		public string UserFullname { get; set; }
		public string Module { get; set; }
		public string TranslatedModule { get; set; }
		public int Instance { get; set; }
		public string Content { get; set; }
		public DateTime fDateTime { get; set; }
		public DateTime DateTime { get; set; }

		public C_Log()
		{
			ID = 0;
			User = 0;
			UserFullname = "";
			Module = "";
			Instance = 0;
			Content = "";
			fDateTime = new DateTime(DateTime.Now.Year, 1, 1);
			DateTime = DateTime.Now;
		}
	}
}
