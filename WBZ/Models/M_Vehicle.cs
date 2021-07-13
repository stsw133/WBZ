namespace WBZ.Models
{
    /// <summary>
    /// Model for Vehicles
    /// </summary>
    public class M_Vehicle : M
    {
        /// Name
        public override string Name => $"{Register} {Brand} {Model}";

        /// Register
        public string Register { get; set; }

        /// Brand
        public string Brand { get; set; }

        /// Model
        public string Model { get; set; }

        /// Capacity
        public decimal? Capacity { get; set; }

        /// Forwarder
        public int ForwarderID { get; set; }
        public string ForwarderName { get; set; }

        /// Driver
        public int DriverID { get; set; }
        public string DriverName { get; set; }

        /// ProdYear
        public int? ProdYear { get; set; }
    }
}
