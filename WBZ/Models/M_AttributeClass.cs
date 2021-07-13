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

        /// Name
        public override string Name { get; set; }

        /// Type
        public string Type { get; set; } = (string)MS_AttributesClasses.Types[0].Value;

        /// DefValue
        public string DefValue { get; set; }

        /// IsRequired
        public bool IsRequired { get; set; }

        /// Values
        public DataTable Values { get; set; } = new DataTable();
    }

    /// <summary>
    /// Model for Attributes
    /// </summary>
    public class M_Attribute
    {
        /// ID
        public long ID { get; set; }

        /// Class
        public M_AttributeClass Class { get; set; } = new M_AttributeClass();

        /// Instance
        public int InstanceID { get; set; }

        /// Value
        public string Value { get; set; }

        /// IsArchival
        public bool IsArchival { get; set; }
    }
}
