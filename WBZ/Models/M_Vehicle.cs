namespace WBZ.Models
{
	public class M_Vehicle : M
	{
		/// <summary>
		/// Name
		/// </summary>
		public override string Name => $"{Register} {Brand} {Model}";

		/// <summary>
		/// Register
		/// </summary>
		public string Register { get; set; } = string.Empty;

		/// <summary>
		/// Brand
		/// </summary>
		public string Brand { get; set; } = string.Empty;

		/// <summary>
		/// Model
		/// </summary>
		public string Model { get; set; } = string.Empty;

		/// <summary>
		/// Capacity
		/// </summary>
		public decimal? Capacity { get; set; } = null;

		/// <summary>
		/// Forwarder
		/// </summary>
		public int ForwarderID { get; set; } = 0;
		public string ForwarderName { get; set; } = string.Empty;

		/// <summary>
		/// Driver
		/// </summary>
		public int DriverID { get; set; } = 0;
		public string DriverName { get; set; } = string.Empty;

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
