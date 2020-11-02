namespace WBZ.Models
{
	public class C_Additional
	{
		public bool Archival { get; set; }
		public string Comment { get; set; }
		public byte[] Icon { get; set; }

		public C_Additional()
		{
			Archival = false;
			Comment = "";
			Icon = null;
		}
	}
}
