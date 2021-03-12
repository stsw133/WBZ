using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Group : M
	{
		public string Module { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public int Instance { get; set; } = 0;
		public int Owner { get; set; } = 0;
		public string Path { get; set; } = string.Empty;

		public string TranslatedModule { get => Global.TranslateModule(Module); }
		public string Fullpath { get => Path + Name; }
	}
}
