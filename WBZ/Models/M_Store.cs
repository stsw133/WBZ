namespace WBZ.Models
{
    /// <summary>
    /// Model for Stores
    /// </summary>
    public class M_Store : M, IMA
    {
        /// IMA
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }

        /// Codename
        public string Codename { get; set; }

        /// Name
        public override string Name { get; set; }

        /// Quantity
        public decimal Quantity { get; set; }

        /// Reserved
        public decimal Reserved { get; set; }
    }
}
