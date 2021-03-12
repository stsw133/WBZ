namespace WBZ.Models
{
	public class M_Store : MA
	{
		public string Codename { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public decimal Amount { get; set; } = 0;
		public decimal Reserved { get; set; } = 0;
	}
}
