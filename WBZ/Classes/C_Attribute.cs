namespace WBZ.Classes
{
	public class C_AttributeClass
	{
		public int ID { get; set; }
		public string Module { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Values { get; set; }
		public string DefValue { get; set; }
		public bool Required { get; set; }
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }

		public C_AttributeClass()
		{
			ID = 0;
			Module = "";
			Name = "";
			Type = "";
			Values = "";
			DefValue = "";
			Required = false;
			Archival = false;
			Comment = "";
			Icon = null;
		}
	}

	public class C_Attribute
	{
		public long ID { get; set; }
		public C_AttributeClass Class { get; set; }
		public int Instance { get; set; }
		public string Value { get; set; }
		public bool Archival { get; set; }

		public C_Attribute()
		{
			ID = 0;
			Class = new C_AttributeClass();
			Instance = 0;
			Value = "";
			Archival = false;
		}
	}
}
