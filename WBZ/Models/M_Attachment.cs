using WBZ.Globals;

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
		public MV cUser { get; set; } = new MV();
		public int User
		{
			get => cUser.ID;
			set => cUser = SQL.ListValues(Config.Modules.USERS, "lastname || ' ' || forename", $"id={value}", false)?[0];
		}

		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => Config.GetModuleTranslation(Module);

		/// <summary>
		/// Instance
		/// </summary>
		public int Instance { get; set; } = 0;

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; set; } = string.Empty;

		/// <summary>
		/// Format
		/// </summary>
		public string Format { get; set; } = string.Empty;

		/// <summary>
		/// Size
		/// </summary>
		public double Size { get; set; } = 0;

		/// <summary>
		/// File
		/// </summary>
		public byte[] File { get; set; } = null;
	}
}
