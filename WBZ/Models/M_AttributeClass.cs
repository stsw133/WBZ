using System.Data;

namespace WBZ.Models
{
	public class M_AttributeClass : M
	{
		/// <summary>
		/// Module
		/// </summary>
		public string Module { get; set; } = string.Empty;
		public string TranslatedModule => TranslateMe.TM.Tr(char.ToUpper(Module[0]) + Module[1..], languageId: StswExpress.Globals.Properties.Language);

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

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
