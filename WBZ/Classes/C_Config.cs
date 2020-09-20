namespace WBZ.Classes
{
	public static class C_Config
	{
		public static string Version;
		public static string Logs_Enabled;
		public static string Email_Host, Email_Port, Email_Address, Email_Password;

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
