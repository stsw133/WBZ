using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Attachment
	{
		public int ID { get; set; }
		public int User { get; set; }
		public string UserFullname { get; set; }
		public string Module { get; set; }
		public int Instance { get; set; }
		public string Name { get; set; }
		public byte[] File { get; set; }

		public M_Attachment()
        {
			UserFullname = string.Empty;
			Module = string.Empty;
			Name = string.Empty;
        }

		public string TranslatedModule
		{
			get
			{
				return Global.TranslateModule(Module);
			}
		}
	}
}
