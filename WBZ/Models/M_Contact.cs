namespace WBZ.Models
{
    /// <summary>
    /// Model for Contacts
    /// </summary>
    public class M_Contact : M, IMM, IMP
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// IMP
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        /// Name
        public override string Name => $"{Lastname} {Forename}";

        /// IsDefault
        public bool IsDefault { get; set; }
    }
}
