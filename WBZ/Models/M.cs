using WBZ.Globals;

namespace WBZ.Models
{
    public class M
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// Archival
        /// </summary>
        public bool Archival { get; set; } = false;

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Icon
        /// </summary>
        public MV cIcon { get; set; } = new MV();
        public int Icon
        {
            get => (int)cIcon.Value;
            set => cIcon = SQL.ListValues(Config.Modules.ICONS, "file", $"id={value}", false)?[0];
        }

        /// <summary>
        /// Group
        /// </summary>
        public int Group { get; set; } = 0;
    }

    public class MA : M
    {
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Postcode
        /// </summary>
        public string Postcode { get; set; } = string.Empty;
    }

    public class MV
    {
        /// <summary>
        /// Display
        /// </summary>
        public object Display { get; set; } = null;

		/// <summary>
		/// Value
		/// </summary>
		public object Value { get; set; } = null;

		/// <summary>
		/// Module
		/// </summary>
		public object Name { get; set; } = null;

		/// <summary>
		/// Tag
		/// </summary>
		public object Tag { get; set; } = null;
	}
}
