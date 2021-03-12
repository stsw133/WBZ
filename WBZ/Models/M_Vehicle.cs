namespace WBZ.Models
{
	public class M_Vehicle : M
	{
		public string Register { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public string Model { get; set; } = string.Empty;
		public decimal? Capacity { get; set; } = null;
		public M_Contractor Forwarder { get; set; } = new M_Contractor();
		public M_Employee Driver { get; set; } = new M_Employee();
		public int? ProdYear { get; set; } = null;

		public string Name { get => $"{Register} {Brand} {Model}"; }
	}
}
