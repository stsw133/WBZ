namespace WBZ.Models
{
	public class M_Icon : M
	{
		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => M_Module.GetModuleTranslation(Module);

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
		public int Height { get; set; } = 0;
		public int Width { get; set; } = 0;

		/// <summary>
		/// File
		/// </summary>
		public byte[] File { get; set; } = null;
	}
}
