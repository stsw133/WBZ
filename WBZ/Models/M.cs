namespace WBZ.Models
{
    /// <summary>
    /// Main (base) model interface
    /// </summary>
    public class M
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Archival
        /// </summary>
        public bool Archival { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public int Group { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        public int Icon { get; set; }
        public byte[] IconContent { get; set; }
    }

    /// <summary>
    /// Model interface with address data
    /// </summary>
    public interface IMA
    {
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Postcode
        /// </summary>
        public string Postcode { get; set; }
    }

    /// <summary>
    /// Model interface with another module reference
    /// </summary>
    public interface IMM
    {
        /// <summary>
        /// Module
        /// </summary>
        public MV Module { get; set; }

        /// <summary>
        /// Instance
        /// </summary>
        public int Instance { get; set; }
    }

    /// <summary>
    /// Model interface with personal data
    /// </summary>
    public interface IMP
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
    }

    /// <summary>
    /// Helper model for simple data like values to ComboBoxes
    /// </summary>
    public class MV
    {
        /// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get; set; } = string.Empty;

		/// <summary>
		/// Value
		/// </summary>
		public object Value { get; set; } = null;

        /// <summary>
        /// Display
        /// </summary>
        public object Display { get; set; } = null;

        /// <summary>
        /// Tag
        /// </summary>
        public object Tag { get; set; } = null;
    }
}
