namespace WBZ.Models
{
	public class M_Attachment
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; } = 0;

		/// <summary>
		/// User
		/// </summary>
		public int User { get; set; } = 0;

		/// <summary>
		/// UserFullname
		/// </summary>
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
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// File
		/// </summary>
		public byte[] File { get; set; } = null;
	}
}
