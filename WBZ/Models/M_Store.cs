namespace WBZ.Models
{
	public class M_Store : MA
	{
		/// <summary>
		/// Codename
		/// </summary>
		public string Codename { get; set; } = string.Empty;

		/// <summary>
		/// Name
		/// </summary>
		public override string Name { get; set; } = string.Empty;

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; set; } = 0;

		/// <summary>
		/// Reserved
		/// </summary>
		public decimal Reserved { get; set; } = 0;
	}
}
