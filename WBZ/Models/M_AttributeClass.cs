using System.Collections.Generic;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
	public class M_AttributeClass : M
	{
		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => Config.GetModuleTranslation(Module);

		/// <summary>
		/// Type
		/// </summary>
		public string Type { get; set; } = string.Empty;

		/// <summary>
		/// DefValue
		/// </summary>
		public string DefValue { get; set; } = string.Empty;

		/// <summary>
		/// Required
		/// </summary>
		public bool Required { get; set; } = false;

		/// <summary>
		/// Values
		/// </summary>
		public DataTable Values { get; set; } = new DataTable();
	}

	public class M_Attribute
	{
		/// <summary>
		/// ID
		/// </summary>
		public long ID { get; set; } = 0;

		/// <summary>
		/// Class
		/// </summary>
		public M_AttributeClass Class { get; set; } = new M_AttributeClass();

		/// <summary>
		/// Instance
		/// </summary>
		public int Instance { get; set; } = 0;

		/// <summary>
		/// Value
		/// </summary>
		public string Value { get; set; } = string.Empty;

		/// <summary>
		/// Archival
		/// </summary>
		public bool Archival { get; set; } = false;
	}

	public static class M_AttributeTypes
	{
		public static List<MV> Types { get; } = new List<MV>()
		{
			new MV() { ID = "string", Value = "Ciąg znaków" },
			new MV() { ID = "date", Value = "Data" },
			new MV() { ID = "int", Value = "Liczba całkowita" },
			new MV() { ID = "double", Value = "Liczba ułamkowa" },
			new MV() { ID = "list", Value = "Lista" }
		};
	}
}
