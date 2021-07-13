namespace WBZ.Models
{
    /// <summary>
    /// Model for Employees
    /// </summary>
    public class M_Employee : M, IMA, IMP
    {
        /// IMA
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }

        /// IMP
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        /// Name
        public override string Name => $"{Lastname} {Forename}";

        /// Department
        public string Department { get; set; }

        /// Position
        public string Position { get; set; }
    }
}
