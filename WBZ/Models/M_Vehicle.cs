using WBZ.Globals;

namespace WBZ.Models
{
	public class M_Vehicle : M
	{
		/// <summary>
		/// Name
		/// </summary>
		public string Name => $"{Register} {Brand} {Model}";

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
		public MV cForwarder { get; set; } = new MV();
		public int Forwarder
		{
			get => cForwarder.ID;
			set => cForwarder = SQL.ListValues(Config.Modules.CONTRACTORS, "codename", $"id={value}", false)?[0];
		}

		/// <summary>
		/// Driver
		/// </summary>
		public MV cDriver { get; set; } = new MV();
		public int Driver
		{
			get => cDriver.ID;
			set => cDriver = SQL.ListValues(Config.Modules.EMPLOYEES, "lastname || ' ' || forename", $"id={value}", false)?[0];
		}

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
