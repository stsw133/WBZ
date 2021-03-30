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
		public int Forwarder { set { cForwarder = SQL.GetInstance<M_Contractor>(Globals.Global.Module.CONTRACTORS, value); } }
		public M_Contractor cForwarder { get; set; } = new M_Contractor();

		/// <summary>
		/// Driver
		/// </summary>
		public int Driver { set { cDriver = SQL.GetInstance<M_Employee>(Globals.Global.Module.EMPLOYEES, value); } }
		public M_Employee cDriver { get; set; } = new M_Employee();

		/// <summary>
		/// ProdYear
		/// </summary>
		public int? ProdYear { get; set; } = null;
	}
}
