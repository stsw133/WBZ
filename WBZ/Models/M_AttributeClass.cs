using System.Collections.Generic;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
	public static class MS_AttributesClasses
	{
		public static List<MV> Types { get; } = new List<MV>()
		{
			new MV() { Value = "string", Display = "Ciąg znaków" },
			new MV() { Value = "date", Display = "Data" },
			new MV() { Value = "int", Display = "Liczba całkowita" },
			new MV() { Value = "double", Display = "Liczba ułamkowa" },
			new MV() { Value = "list", Display = "Lista" }
		};
	}

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
}
