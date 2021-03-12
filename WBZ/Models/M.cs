namespace WBZ.Models
{
    public class M
    {
        public int ID { get; set; } = 0;
        public bool Archival { get; set; } = false;
        public string Comment { get; set; } = string.Empty;
        public byte[] Icon { get; set; } = null;
        public int Group { get; set; } = 0;
    }

    public class MA : M
    {
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
    }
}
