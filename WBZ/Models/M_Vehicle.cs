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
		public MV Forwarder { get; set; } = new MV();
		public int ForwarderID
		{
			get => (int)(Forwarder.ID ?? 0);
            set => Forwarder.ID = value;
		}
		public string ForwarderName
		{
			get => (string)Forwarder.Value;
			set => Forwarder.Value = value;
		}

		/// <summary>
		/// Driver
		/// </summary>
		public MV Driver { get; set; } = new MV();
		public int DriverID
		{
			get => (int)(Driver.ID ?? 0);
			set => Driver.ID = value;
		}
		public string DriverName
		{
			get => (string)Driver.Value;
			set => Driver.Value = value;
		}

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
