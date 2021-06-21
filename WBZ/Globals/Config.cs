using StswExpress;
using StswExpress.Translate;
using System.Collections.Generic;
using WBZ.Models;

namespace WBZ.Globals
{
	public static class Config
	{
		/// Newest version to download
		public static string VersionNewest { get; set; } = null;

		/// Logged user
		public static M_User User { get; set; } = new M_User();

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
        /// List of modules
        /// </summary>
        public static List<MV> ListModules { get; } = new List<MV>()
        {
            new MV()
			{
				Name = string.Empty,
				Alias = string.Empty,
				Value = string.Empty,
				Display = string.Empty
			},
            new MV()
			{
				Name = nameof(Modules.Articles),
				Alias = "art",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Articles), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Articles))
			},
            new MV()
			{
				Name = nameof(Modules.AttributesClasses),
				Alias = "atc",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.AttributesClasses), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.AttributesClasses))
			},/*
            new MV()
			{
				Name = nameof(Modules.Complaints),
				Alias = "cpl",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Complaints), '_').ToLower(),
				Display = GetModuleTranslation(nameof(Modules.Complaints))
			},*/
            new MV()
			{
				Name = nameof(Modules.Contractors),
				Alias = "cnt",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Contractors), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Contractors))
			},
            new MV()
			{
				Name = nameof(Modules.Distributions),
				Alias = "dis",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Distributions), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Distributions))
			},
            new MV()
			{
				Name = nameof(Modules.Documents),
				Alias = "doc",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Documents), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Documents))
			},
            new MV()
			{
				Name = nameof(Modules.Employees),
				Alias = "emp",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Employees), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Employees))
			},
            new MV()
			{
				Name = nameof(Modules.Families),
				Alias = "fam",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Families), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Families))
			},/*
            new MV()
			{
				Module = nameof(Modules.Gallery),
				Alias = "gal",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Gallery), '_').ToLower(),
				Display = GetModuleTranslation(nameof(Modules.Gallery))
			},*/
            new MV()
			{
				Name = nameof(Modules.Icons),
				Alias = "ico",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Icons), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Icons))
			},/*
            new MV()
			{
				Module = nameof(Modules.Orders),
				Alias = "ord",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Orders), '_').ToLower(),
				Display = GetModuleTranslation(nameof(Modules.Orders))
			},*/ /*
            new MV()
			{
				Module = nameof(Modules.Shipments),
				Alias = "shi",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Shipments), '_').ToLower(),
				Display = GetModuleTranslation(nameof(Modules.Shipments))
			},*/
            new MV()
			{
				Name = nameof(Modules.Stores),
				Alias = "sto",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Stores), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Stores))
			},
            new MV()
			{
				Name = nameof(Modules.Users),
				Alias = "use",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Users), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Users))
			},
            new MV()
			{
				Name = nameof(Modules.Vehicles),
				Alias = "veh",
				Value = Fn.AddCharBeforeUpperLetters(nameof(Modules.Vehicles), '_').ToLower(),
				Display = TM.Tr(nameof(Modules.Vehicles))
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
				Alias = "att",
				Value = Fn.AddCharBeforeUpperLetters("Attachments", '_').ToLower(),
				Display = TM.Tr("Attachments")
			},
            new MV()
			{
				Name = "Attributes",
				Alias = "atr",
				Value = Fn.AddCharBeforeUpperLetters("Attributes", '_').ToLower(),
				Display = TM.Tr("Attributes")
			},
			new MV()
			{
				Name = "Community",
				Alias = "com",
				Value = Fn.AddCharBeforeUpperLetters("Community", '_').ToLower(),
				Display = TM.Tr("Community")
			},
			new MV()
			{
				Name = "Contacts",
				Alias = "con",
				Value = Fn.AddCharBeforeUpperLetters("Contacts", '_').ToLower(),
				Display = TM.Tr("Contacts")
			},
            new MV()
			{
				Name = "Filters",
				Alias = "fil",
				Value = Fn.AddCharBeforeUpperLetters("Filters", '_').ToLower(),
				Display = TM.Tr("Filters")
			},
            new MV()
			{
				Name = "Groups",
				Alias = "gro",
				Value = Fn.AddCharBeforeUpperLetters("Groups", '_').ToLower(),
				Display = TM.Tr("Groups")
			},
            new MV()
			{
				Name = "Logs",
				Alias = "log",
				Value = Fn.AddCharBeforeUpperLetters("Logs", '_').ToLower(),
				Display = TM.Tr("Logs")
			},
            new MV()
			{
				Name = "Translations",
				Alias = "tns",
				Value = Fn.AddCharBeforeUpperLetters("Translations", '_').ToLower(),
				Display = TM.Tr("Translations")
			}
        };
		public static MV GetModule(string moduleName) => ListModules.Find(x => x.Name == moduleName) ?? ListSubModules.Find(x => x.Name == moduleName);

		/// <summary>
		/// UserPermType
		/// </summary>
		public enum PermType
		{
			PREVIEW, SAVE, DELETE, GROUPS, FILTERS, STATS  //TODO - STATS do dodania w wersji 1.3.0
		}
	}
}
