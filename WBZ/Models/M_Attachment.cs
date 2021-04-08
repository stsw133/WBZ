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
		public int User { set => cUser = SQL.ComboInstances(Globals.Global.Module.USERS, "lastname || ' ' || forename", $"id={value}", false)?[0]; }
		public M_ComboValue cUser { get; set; } = new M_ComboValue();

		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => Globals.Global.GetModuleTranslation(Module);

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
