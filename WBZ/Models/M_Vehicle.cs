namespace WBZ.Models
{
	public class M_Vehicle : M
	{
		public string Register { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public decimal? Capacity { get; set; }
		public M_Contractor Forwarder { get; set; }
		public M_Employee Driver { get; set; }
		public int? ProdYear { get; set; }

		public M_Vehicle()
        {
			Register = string.Empty;
			Brand = string.Empty;
			Model = string.Empty;
			Forwarder = new M_Contractor();
			Driver = new M_Employee();
        }

		public string Name
		{
			get
			{
				return $"{Register} {Brand} {Model}";
			}
		}
	}
}
