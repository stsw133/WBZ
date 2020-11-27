using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Group : M
	{
		public string Module { get; set; }
		public string Name { get; set; }
		public int Instance { get; set; }
		public int Owner { get; set; }
		public string Path { get; set; }

		public M_Group()
        {
			Module = string.Empty;
			Name = string.Empty;
			Path = string.Empty;
        }

		public string TranslatedModule
		{
			get
			{
				return Global.TranslateModule(Module);
			}
		}
		public string Fullpath
		{
			get
			{
				return Path + Name;
			}
		}
	}
}
