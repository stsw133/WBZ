using System;

namespace WBZ.Models
{
    public class M
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; } = 0;

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
        public int Icon
        {
            get => cIcon.Item1 ?? 0;
            set => cIcon = new Tuple<int?, byte[]>(value, SQL.GetInstance<M_Icon>(Globals.Config.Modules.ICONS, value)?.File);
        }
        public Tuple<int?, byte[]> cIcon { get; set; } = new Tuple<int?, byte[]>(null, null);

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
}
