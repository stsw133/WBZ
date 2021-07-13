namespace WBZ.Models
{
    /// <summary>
    /// Model for Contractors
    /// </summary>
    public class M_Contractor : M, IMA
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

        /// Branch
        public string Branch { get; set; }

        /// NIP
        public string NIP { get; set; }

        /// REGON
        public string REGON { get; set; }
    }
}
