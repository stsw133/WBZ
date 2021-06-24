namespace WBZ.Models
{
    /// <summary>
    /// Model for Contacts
    /// </summary>
    public class M_Contact : M, IMM, IMP
    {
        /// IMM
        public MV Module { get; set; }
        public int Instance { get; set; }

        /// IMP
        public override string Name => Fullname;
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Fullname => $"{Lastname} {Forename}";
        public string Email { get; set; }
        public string Phone { get; set; }

        /// <summary>
        /// IsDefault
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
