namespace WBZ.Models
{
	public static class M_Config
	{
		public static string Version { get => SQL.GetPropertyValue("VERSION"); }
		public static string Logs_Enabled { get => SQL.GetPropertyValue("LOGS_ENABLED"); }
		public static string Email_Host { get => SQL.GetPropertyValue("EMAIL_HOST"); }
		public static string Email_Port { get => SQL.GetPropertyValue("EMAIL_PORT"); }
		public static string Email_Address { get => SQL.GetPropertyValue("EMAIL_ADDRESS"); }
		public static string Email_Password { get => SQL.GetPropertyValue("EMAIL_PASSWORD"); }
	}
}
