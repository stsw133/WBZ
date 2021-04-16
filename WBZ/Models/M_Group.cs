using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Group : M
	{
		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => Config.GetModuleTranslation(Module);

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Instance
		/// </summary>
		public int Instance { get; set; } = 0;

		/// <summary>
		/// Owner
		/// </summary>
		public int Owner { get; set; } = 0;

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; set; } = string.Empty;
		public string Fullpath => Path + Name;
	}
}
