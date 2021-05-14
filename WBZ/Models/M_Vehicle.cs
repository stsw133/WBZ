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
		private MV forwarder { get; set; } = new MV();
		public int ForwarderID
		{
			get => (int)forwarder.ID;
            set => forwarder.ID = value;
		}
		public string ForwarderName
		{
			get => (string)forwarder.Value;
			set => forwarder.Value = value;
		}

		/// <summary>
		/// Driver
		/// </summary>
		private MV driver { get; set; } = new MV();
		public int DriverID
		{
			get => (int)driver.ID;
			set => driver.ID = value;
		}
		public string DriverName
		{
			get => (string)driver.Value;
			set => driver.Value = value;
		}

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
