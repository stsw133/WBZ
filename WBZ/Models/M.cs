namespace WBZ.Models
{
    public class M
    {
        public int ID { get; set; }
        public bool Archival { get; set; }
        public string Comment { get; set; }
        public byte[] Icon { get; set; }
        public int Group { get; set; }

        public M()
        {
            Comment = string.Empty;
        }
    }

    public class MA : M
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }

        public MA()
        {
            Address = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            Postcode = string.Empty;
        }
    }
}
