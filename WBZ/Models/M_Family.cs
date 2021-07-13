using System;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Families
    /// </summary>
    public class M_Family : M, IMA
    {
        /// IMA
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }

        /// Name
        public override string Name { get; set; }

        /// Declarant
        public string Declarant { get; set; }

        /// Members
        public short? Members { get; set; }

        /// Status
        public bool Status { get; set; }

        /// C_*
        public bool C_SMS { get; set; }
        public bool C_Call { get; set; }
        public bool C_Email { get; set; }

        /// Donation
        public DateTime? DonationLast { get; set; }
        public decimal DonationWeight { get; set; }
    }
}
