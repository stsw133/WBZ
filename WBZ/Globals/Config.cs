using StswExpress;
using System.Collections.Generic;
using System.Linq;
using WBZ.Models;

namespace WBZ.Globals
{
	public static class Config
	{
		public static string Version => SQL.GetPropertyValue("VERSION", Fn.AppVersion());
		public static string Attachment_Size_Max => SQL.GetPropertyValue("ATTACHMENT_SIZE_MAX", (1024*1024*10).ToString());
		public static string Email_Host => SQL.GetPropertyValue("EMAIL_HOST", "smtp.gmail.com");
		public static string Email_Port => SQL.GetPropertyValue("EMAIL_PORT", 587.ToString());
		public static string Email_Address => SQL.GetPropertyValue("EMAIL_ADDRESS", string.Empty);
		public static string Email_Password => SQL.GetPropertyValue("EMAIL_PASSWORD", string.Empty);
		public static string Icon_Dimensions_Max => SQL.GetPropertyValue("ICON_DIMENSIONS_MAX", 32.ToString());
		public static string Icon_Size_Max => SQL.GetPropertyValue("ICON_SIZE_MAX", (1024 * 10).ToString());
		public static string Logs_Enabled => SQL.GetPropertyValue("LOGS_ENABLED", 0.ToString());

        /// <summary>
        /// Modules
        /// </summary>
        public static class Modules
        {
            public const string ARTICLES = "articles";
            public const string ATTRIBUTES_CLASSES = "attributes_classes";
            //public const string COMMUNITY = "community";  // do dodania w wersji 1.3.0
            public const string CONTRACTORS = "contractors";
            public const string DISTRIBUTIONS = "distributions";
            public const string DOCUMENTS = "documents";
            public const string EMPLOYEES = "employees";
            public const string FAMILIES = "families";
            public const string ICONS = "icons";
            //public const string ORDERS = "orders";  // do dodania w wersji 1.3.0
            //public const string SHIPMENTS = "shipments";  // do dodania w wersji 1.3.0
            public const string STATS = "stats";    // do ulepszenia w wersji 1.3.0
            public const string STORES = "stores";
            public const string USERS = "users";
            public const string VEHICLES = "vehicles";
        }
        public static class SubModules
        {
            public const string ATTACHMENTS = "attachments";
            public const string ATTRIBUTES = "attributes";
            public const string CONTACTS = "contacts";
            public const string GROUPS = "groups";
            public const string LOGS = "logs";
        }
        public static string GetModuleAlias(string module) => string.Join(string.Empty, module.Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(y => y[0]));
        public static string GetModuleTranslation(string module) => StswExpress.Translate.TM.Tr(string.Join("", module.Split('_').Select(x => x.Substring(0, 1).ToUpper() + x[1..]).ToArray()));

        /// <summary>
        /// List
        /// </summary>
        public static List<MV> ListModules { get; } = new List<MV>()
        {
            new MV() { ID = string.Empty, Value = string.Empty },
            new MV() { ID = Modules.ARTICLES, Value = GetModuleTranslation(Modules.ARTICLES) },
            new MV() { ID = Modules.ATTRIBUTES_CLASSES, Value = GetModuleTranslation(Modules.ATTRIBUTES_CLASSES) },
          //new MV() { ID = Module.COMMUNITY, Value = GetModuleTranslation(Module.COMMUNITY) },
            new MV() { ID = Modules.CONTRACTORS, Value = GetModuleTranslation(Modules.CONTRACTORS) },
            new MV() { ID = Modules.DISTRIBUTIONS, Value = GetModuleTranslation(Modules.DISTRIBUTIONS) },
            new MV() { ID = Modules.DOCUMENTS, Value = GetModuleTranslation(Modules.DOCUMENTS) },
            new MV() { ID = Modules.EMPLOYEES, Value = GetModuleTranslation(Modules.EMPLOYEES) },
            new MV() { ID = Modules.FAMILIES, Value = GetModuleTranslation(Modules.FAMILIES) },
            new MV() { ID = Modules.ICONS, Value = GetModuleTranslation(Modules.ICONS) },
          //new MV() { ID = Module.STATS, Value = GetModuleTranslation(Module.STATS) },
            new MV() { ID = Modules.STORES, Value = GetModuleTranslation(Modules.STORES) },
            new MV() { ID = Modules.USERS, Value = GetModuleTranslation(Modules.USERS) },
            new MV() { ID = Modules.VEHICLES, Value = GetModuleTranslation(Modules.VEHICLES) }
        };
        public static List<MV> ListSubModules = new List<MV>()
        {
            new MV() { ID = SubModules.ATTACHMENTS, Value = GetModuleTranslation(SubModules.ATTACHMENTS) },
            new MV() { ID = SubModules.ATTRIBUTES, Value = GetModuleTranslation(SubModules.ATTRIBUTES) },
            new MV() { ID = SubModules.CONTACTS, Value = GetModuleTranslation(SubModules.CONTACTS) },
            new MV() { ID = SubModules.GROUPS, Value = GetModuleTranslation(SubModules.GROUPS) },
            new MV() { ID = SubModules.LOGS, Value = GetModuleTranslation(SubModules.LOGS) }
        };
    }
}
