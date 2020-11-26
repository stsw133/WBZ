using WBZ.Globals;

namespace WBZ.Models
{
	public class M_AttributeClass : M
	{
		public string Module { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Values { get; set; }
		public string DefValue { get; set; }
		public bool Required { get; set; }

		public M_AttributeClass()
        {
			Module = string.Empty;
			Name = string.Empty;
			Type = string.Empty;
			Values = string.Empty;
			DefValue = string.Empty;
        }

		public string TranslatedModule
		{
			get
			{
				return Global.TranslateModule(Module);
			}
		}
	}

	public class M_Attribute
	{
		public long ID { get; set; }
		public M_AttributeClass Class { get; set; }
		public int Instance { get; set; }
		public string Value { get; set; }
		public bool Archival { get; set; }

		public M_Attribute()
		{
			Class = new M_AttributeClass();
			Value = string.Empty;
		}
	}
}
