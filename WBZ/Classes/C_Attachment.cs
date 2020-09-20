namespace WBZ.Classes
{
	public class C_Attachment
	{
		public int ID { get; set; }
		public int User { get; set; }
		public string Module { get; set; }
		public int Instance { get; set; }
		public string Name { get; set; }
		public byte[] File { get; set; }

		public C_Attachment()
		{
			ID = 0;
			User = 0;
			Module = "";
			Instance = 0;
			Name = "";
			File = null;
		}
	}
}
