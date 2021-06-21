using System.Collections.Generic;
using System.Data;

namespace WBZ.Models
{
	/// <summary>
	/// Static model for AttributesClasses sources
	/// </summary>
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

	/// <summary>
	/// Model for AttributesClasses
	/// </summary>
	public class M_AttributeClass : M, IMM
	{
		/// IMM
		public MV Module { get; set; }
		public int InstanceID { get; set; }

		/// <summary>
		/// Type
		/// </summary>
		public string Type { get; set; } = MS_AttributesClasses.Types[0].Value.ToString();

		/// <summary>
		/// DefValue
		/// </summary>
		public string DefValue { get; set; }

		/// <summary>
		/// Required
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Values
		/// </summary>
		public DataTable Values { get; set; } = new DataTable();
    }

	/// <summary>
	/// Model for Attributes
	/// </summary>
	public class M_Attribute
	{
		/// <summary>
		/// ID
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// Class
		/// </summary>
		public M_AttributeClass Class { get; set; } = new M_AttributeClass();

		/// <summary>
		/// InstanceID
		/// </summary>
		public int InstanceID { get; set; }

		/// <summary>
		/// Value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Archival
		/// </summary>
		public bool Archival { get; set; }
	}
}
