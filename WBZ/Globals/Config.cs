using StswExpress;
using StswExpress.Translate;
using System.Collections.Generic;
using System.Linq;
using WBZ.Models;

namespace WBZ.Globals
{
	public static class Config
	{
		/// Config
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
            public const string CONTRACTORS = "contractors";
            public const string DISTRIBUTIONS = "distributions";
            public const string DOCUMENTS = "documents";
            public const string EMPLOYEES = "employees";
            public const string FAMILIES = "families";
			public const string GALLERY = "gallery";  // do dodania w wersji 1.3.0
			public const string ICONS = "icons";
            public const string ORDERS = "orders";  // do dodania w wersji 1.3.0
            public const string SHIPMENTS = "shipments";  // do dodania w wersji 1.3.0
            public const string STORES = "stores";
            public const string USERS = "users";
            public const string VEHICLES = "vehicles";

			public const string ATTACHMENTS = "attachments";
			public const string ATTRIBUTES = "attributes";
			public const string COMMUNITY = "community";  // do dodania w wersji 1.3.0
			public const string CONTACTS = "contacts";
			public const string FILTERS = "filters";
			public const string GROUPS = "groups";
			public const string LOGS = "logs";
			public const string TRANSLATIONS = "translations";  // do dodania w wersji 1.4.0
		}
		public static string GetModuleAlias(string module) => string.Join(string.Empty, module.Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(y => y[0]));
        public static string GetModuleTranslation(string module) => TM.Tr(string.Join("", module.Split('_').Select(x => x.Substring(0, 1).ToUpper() + x[1..]).ToArray()));

        /// <summary>
        /// List of modules
        /// </summary>
        public static List<MV> ListModules { get; } = new List<MV>()
        {
            new MV()
			{
				Name = string.Empty,
				Tag = string.Empty,
				Value = string.Empty,
				Display = string.Empty
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Articles),
				Tag = "art",
				Value = Modules.ARTICLES,
				Display = GetModuleTranslation(Modules.ARTICLES)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.AttributesClasses),
				Tag = "atc",
				Value = Modules.ATTRIBUTES_CLASSES,
				Display = GetModuleTranslation(Modules.ATTRIBUTES_CLASSES)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Contractors),
				Tag = "cnt",
				Value = Modules.CONTRACTORS,
				Display = GetModuleTranslation(Modules.CONTRACTORS)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Distributions),
				Tag = "dis",
				Value = Modules.DISTRIBUTIONS,
				Display = GetModuleTranslation(Modules.DISTRIBUTIONS)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Documents),
				Tag = "doc",
				Value = Modules.DOCUMENTS,
				Display = GetModuleTranslation(Modules.DOCUMENTS)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Employees),
				Tag = "emp",
				Value = Modules.EMPLOYEES,
				Display = GetModuleTranslation(Modules.EMPLOYEES)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Families),
				Tag = "fam",
				Value = Modules.FAMILIES,
				Display = GetModuleTranslation(Modules.FAMILIES)
			},/*
            new MV()
			{
				Module = nameof(WBZ.Modules.Gallery),
				Tag = "gal",
				Value = Modules.GALLERY,
				Display = GetModuleTranslation(Modules.GALLERY)
			},*/
            new MV()
			{
				Name = nameof(WBZ.Modules.Icons),
				Tag = "ico",
				Value = Modules.ICONS,
				Display = GetModuleTranslation(Modules.ICONS)
			},/*
            new MV()
			{
				Module = nameof(WBZ.Modules.Orders),
				Tag = "ord",
				Value = Modules.ORDERS,
				Display = GetModuleTranslation(Modules.ORDERS)
			},*/ /*
            new MV()
			{
				Module = nameof(WBZ.Modules.Shipments),
				Tag = "shi",
				Value = Modules.SHIPMENTS,
				Display = GetModuleTranslation(Modules.SHIPMENTS)
			},*/
            new MV()
			{
				Name = nameof(WBZ.Modules.Stores),
				Tag = "sto",
				Value = Modules.STORES,
				Display = GetModuleTranslation(Modules.STORES)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Users),
				Tag = "use",
				Value = Modules.USERS,
				Display = GetModuleTranslation(Modules.USERS)
			},
            new MV()
			{
				Name = nameof(WBZ.Modules.Vehicles),
				Tag = "veh",
				Value = Modules.VEHICLES,
				Display = GetModuleTranslation(Modules.VEHICLES)
			}
        };
		/// <summary>
		/// List of sub-modules
		/// </summary>
		public static List<MV> ListSubModules = new List<MV>()
        {
            new MV()
			{
				Name = "Attachments",
				Tag = "att",
				Value = Modules.ATTACHMENTS,
				Display = GetModuleTranslation(Modules.ATTACHMENTS)
			},
            new MV()
			{
				Name = "Attributes",
				Tag = "atr",
				Value = Modules.ATTRIBUTES,
				Display = GetModuleTranslation(Modules.ATTRIBUTES)
			},
			new MV()
			{
				Name = "Community",
				Tag = "com",
				Value = Modules.COMMUNITY,
				Display = GetModuleTranslation(Modules.COMMUNITY)
			},
			new MV()
			{
				Name = "Contacts",
				Tag = "con",
				Value = Modules.CONTACTS,
				Display = GetModuleTranslation(Modules.CONTACTS)
			},
            new MV()
			{
				Name = "Filters",
				Tag = "fil",
				Value = Modules.FILTERS,
				Display = GetModuleTranslation(Modules.FILTERS)
			},
            new MV()
			{
				Name = "Groups",
				Tag = "gro",
				Value = Modules.GROUPS,
				Display = GetModuleTranslation(Modules.GROUPS)
			},
            new MV()
			{
				Name = "Logs",
				Tag = "log",
				Value = Modules.LOGS,
				Display = GetModuleTranslation(Modules.LOGS)
			},
            new MV()
			{
				Name = "Translations",
				Tag = "trs",
				Value = Modules.TRANSLATIONS,
				Display = GetModuleTranslation(Modules.TRANSLATIONS)
			}
        };
    }
}
