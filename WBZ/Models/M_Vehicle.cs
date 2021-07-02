namespace WBZ.Models
{
    /// <summary>
    /// Model for Vehicles
    /// </summary>
    public class M_Vehicle : M
    {
        /// <summary>
        /// Name
        /// </summary>
        public override string Name => $"{Register} {Brand} {Model}";

        /// <summary>
        /// Register
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Capacity
        /// </summary>
        public decimal? Capacity { get; set; }

        /// <summary>
        /// Forwarder
        /// </summary>
        public int ForwarderID { get; set; }
        public string ForwarderName { get; set; }

        /// <summary>
        /// Driver
        /// </summary>
        public int DriverID { get; set; }
        public string DriverName { get; set; }

        /// <summary>
        /// ProdYear
        /// </summary>
        public int? ProdYear { get; set; }
    }
}
