using StswExpress;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WBZ.Globals
{
	public static class Config
	{
		public static string Version => SQL.GetPropertyValue("VERSION", Fn.AppVersion());
		public static string Email_Host => SQL.GetPropertyValue("EMAIL_HOST", "smtp.gmail.com");
		public static string Email_Port => SQL.GetPropertyValue("EMAIL_PORT", 587.ToString());
		public static string Email_Address => SQL.GetPropertyValue("EMAIL_ADDRESS", "wbz.email.testowy@gmail.com");
		public static string Email_Password => SQL.GetPropertyValue("EMAIL_PASSWORD", string.Empty);
		public static string Attachment_Size_Max => SQL.GetPropertyValue("ATTACHMENT_SIZE_MAX", (1024*1024*10).ToString());
		public static string Icon_Dimensions_Max => SQL.GetPropertyValue("ICON_DIMENSIONS_MAX", 32.ToString());
		public static string Icon_Size_Max => SQL.GetPropertyValue("ICON_SIZE_MAX", (1024 * 10).ToString());
		public static string Logs_Enabled => SQL.GetPropertyValue("LOGS_ENABLED", 0.ToString());

        /// <summary>
        /// Modules
        /// </summary>
        public static class Modules
        {
            public const string ARTICLES = "articles";
            public const string ATTACHMENTS = "attachments";
            public const string ATTRIBUTES = "attributes";  // w wersji 1.2.0 przerobienie na submoduł
            public const string ATTRIBUTES_CLASSES = "attributes_classes";
            //public const string COMMUNITY = "community";  // do dodania w wersji 1.3.0
            public const string CONTRACTORS = "contractors";
            public const string CONTACTS = "contacts";  // w wersji 1.2.0 przerobienie na submoduł
            public const string DISTRIBUTIONS = "distributions";
            public const string DOCUMENTS = "documents";
            public const string EMPLOYEES = "employees";
            public const string FAMILIES = "families";
            public const string GROUPS = "groups";
            public const string ICONS = "icons";
            public const string LOGIN = "login";
            public const string LOGS = "logs";
            //public const string ORDERS = "orders";  // do dodania w wersji 1.3.0
            //public const string SHIPMENTS = "shipments";  // do dodania w wersji 1.3.0
            public const string STATS = "stats";    // do ulepszenia w wersji 1.3.0
            public const string STORES = "stores";
            public const string USERS = "users";
            public const string VEHICLES = "vehicles";
        }
        public static string GetModuleAlias(string module) => string.Join(string.Empty, module.Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(y => y[0]));
        public static string GetModuleTranslation(string module) => StswExpress.Translate.TM.Tr(string.Join("", module.Split('_').Select(x => x.Substring(0, 1).ToUpper() + x[1..]).ToArray()));

        /// <summary>
        /// List
        /// </summary>
        public static List<Tuple<string, string>> ListModules = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>(string.Empty, string.Empty),
            new Tuple<string, string>(Modules.ARTICLES, GetModuleTranslation(Modules.ARTICLES)),
            new Tuple<string, string>(Modules.ATTACHMENTS, GetModuleTranslation(Modules.ATTACHMENTS)),
            //new Tuple<string, string>(Module.ATTRIBUTES, GetModuleTranslation(Module.ATTRIBUTES)),
            new Tuple<string, string>(Modules.ATTRIBUTES_CLASSES, GetModuleTranslation(Modules.ATTRIBUTES_CLASSES)),
            //new Tuple<string, string>(Module.COMMUNITY, GetModuleTranslation(Module.COMMUNITY)),
            new Tuple<string, string>(Modules.CONTRACTORS, GetModuleTranslation(Modules.CONTRACTORS)),
            //new Tuple<string, string>(Module.CONTACTS, GetModuleTranslation(Module.CONTACTS)),
            new Tuple<string, string>(Modules.DISTRIBUTIONS, GetModuleTranslation(Modules.DISTRIBUTIONS)),
            new Tuple<string, string>(Modules.DOCUMENTS, GetModuleTranslation(Modules.DOCUMENTS)),
            new Tuple<string, string>(Modules.EMPLOYEES, GetModuleTranslation(Modules.EMPLOYEES)),
            new Tuple<string, string>(Modules.FAMILIES, GetModuleTranslation(Modules.FAMILIES)),
            //new Tuple<string, string>(Module.GROUPS, GetModuleTranslation(Module.GROUPS)),
            new Tuple<string, string>(Modules.ICONS, GetModuleTranslation(Modules.ICONS)),
            //new Tuple<string, string>(Modules.LOGIN, GetModuleTranslation(Modules.LOGIN)),
            new Tuple<string, string>(Modules.LOGS, GetModuleTranslation(Modules.LOGS)),
            //new Tuple<string, string>(Module.STATS, GetModuleTranslation(Module.STATS)),
            new Tuple<string, string>(Modules.STORES, GetModuleTranslation(Modules.STORES)),
            new Tuple<string, string>(Modules.USERS, GetModuleTranslation(Modules.USERS)),
            new Tuple<string, string>(Modules.VEHICLES, GetModuleTranslation(Modules.VEHICLES)),
        };
    }
}
