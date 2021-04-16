using StswExpress.Globals;

namespace WBZ.Models
{
	public static class M_Config
	{
		public static string Version => SQL.GetPropertyValue("VERSION", Global.AppVersion());
		public static string Email_Host => SQL.GetPropertyValue("EMAIL_HOST", "smtp.gmail.com");
		public static string Email_Port => SQL.GetPropertyValue("EMAIL_PORT", 587.ToString());
		public static string Email_Address => SQL.GetPropertyValue("EMAIL_ADDRESS", "wbz.email.testowy@gmail.com");
		public static string Email_Password => SQL.GetPropertyValue("EMAIL_PASSWORD", string.Empty);
		public static string Attachment_Size_Max => SQL.GetPropertyValue("ATTACHMENT_SIZE_MAX", (1024*1024*10).ToString());
		public static string Icon_Dimensions_Max => SQL.GetPropertyValue("ICON_DIMENSIONS_MAX", 64.ToString());
		public static string Icon_Size_Max => SQL.GetPropertyValue("ICON_SIZE_MAX", (1024 * 10).ToString());
		public static string Logs_Enabled => SQL.GetPropertyValue("LOGS_ENABLED", 0.ToString());
	}
}
