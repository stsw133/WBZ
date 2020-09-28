namespace WBZ.Classes
{
    public class C_Contact
    {
        public int ID { get; set; }
        public string Module { get; set; }
        public int Instance { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public bool Default { get; set; }
        public bool Archival { get; set; }

		public C_Contact()
		{
			ID = 0;
			Module = "";
            Instance = 0;
            Email = "";
            Phone = "";
            Forename = "";
            Lastname = "";
            Default = false;
			Archival = false;
		}
	}
}
