namespace WBZ.Models
{
	public class M_Attachment
	{
		public int ID { get; set; } = 0;
		public int User { get; set; } = 0;
		public string UserFullname { get; set; } = string.Empty;
		public string Module { get; set; } = string.Empty;
		public int Instance { get; set; } = 0;
		public string Name { get; set; } = string.Empty;
		public byte[] File { get; set; } = null;

		public string TranslatedModule { get => TranslateMe.TM.Tr(char.ToUpper(Module[0]) + Module[1..], languageId: StswExpress.Globals.Properties.Language); }
	}
}
