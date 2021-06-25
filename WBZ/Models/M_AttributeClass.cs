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
		public string Type { get; set; } = (string)MS_AttributesClasses.Types[0].Value;

		/// <summary>
		/// DefValue
		/// </summary>
		public string DefValue { get; set; }

		/// <summary>
		/// IsRequired
		/// </summary>
		public bool IsRequired { get; set; }

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
		/// Instance
		/// </summary>
		public int InstanceID { get; set; }

		/// <summary>
		/// Value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// IsArchival
		/// </summary>
		public bool IsArchival { get; set; }
	}
}
