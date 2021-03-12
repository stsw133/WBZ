using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
	public class M_AttributeClass : M
	{
		public string Module { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
		public string DefValue { get; set; } = string.Empty;
		public bool Required { get; set; } = false;
		public DataTable Values { get; set; } = new DataTable();

		public string TranslatedModule { get => Global.TranslateModule(Module); }
	}

	public class M_Attribute
	{
		public long ID { get; set; } = 0;
		public M_AttributeClass Class { get; set; } = new M_AttributeClass();
		public int Instance { get; set; } = 0;
		public string Value { get; set; } = string.Empty;
		public bool Archival { get; set; } = false;
	}
}
