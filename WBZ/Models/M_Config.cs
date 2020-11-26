namespace WBZ.Models
{
	public static class M_Config
	{
		public static string Version { get; set; }
		public static string Logs_Enabled { get; set; }
		public static string Email_Host { get; set; }
		public static string Email_Port { get; set; }
		public static string Email_Address { get; set; }
		public static string Email_Password { get; set; }

		public static void LoadConfig()
		{
			Version = SQL.GetPropertyValue("VERSION");
			Logs_Enabled = SQL.GetPropertyValue("LOGS_ENABLED");
			Email_Host = SQL.GetPropertyValue("EMAIL_HOST");
			Email_Port = SQL.GetPropertyValue("EMAIL_PORT");
			Email_Address = SQL.GetPropertyValue("EMAIL_ADDRESS");
			Email_Password = SQL.GetPropertyValue("EMAIL_PASSWORD");
		}
	}
}
