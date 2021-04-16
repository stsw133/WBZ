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
		public int Forwarder { set => cForwarder = SQL.ComboInstances(Config.Modules.CONTRACTORS, "codename", $"id={value}", false)?[0]; }
		public M_ComboValue cForwarder { get; set; } = new M_ComboValue();

		/// <summary>
		/// Driver
		/// </summary>
		public int Driver { set => cDriver = SQL.ComboInstances(Config.Modules.EMPLOYEES, "lastname || ' ' || forename", $"id={value}", false)?[0]; }
		public M_ComboValue cDriver { get; set; } = new M_ComboValue();

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
