namespace WBZ.Models
{
	public class M_Store : MA
	{
		public string Codename { get; set; }
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public decimal Reserved { get; set; }

		public M_Store()
        {
			Codename = string.Empty;
			Name = string.Empty;
        }
	}
}
