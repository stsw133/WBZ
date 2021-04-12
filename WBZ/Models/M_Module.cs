using System;
using System.Collections.Generic;
using System.Linq;

namespace WBZ.Models
{
    public static class M_Module
    {
        /// <summary>
        /// Constants
        /// </summary>
        public static class Module
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
            public const string LOGS = "logs";
            //public const string ORDERS = "orders";  // do dodania w wersji 1.3.0
            //public const string SHIPMENTS = "shipments";  // do dodania w wersji 1.3.0
            public const string STATS = "stats";    // do ulepszenia w wersji 1.3.0
            public const string STORES = "stores";
            public const string USERS = "users";
            public const string VEHICLES = "vehicles";
        }
        public static string GetModuleAlias(string module) => string.Join(string.Empty, module.Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(y => y[0]));
        public static string GetModuleTranslation(string module) => TranslateMe.TM.Tr(string.Join("", module.Split('_').Select(x => x.Substring(0, 1).ToUpper() + x[1..]).ToArray()), languageId: StswExpress.Globals.Properties.Language);

        /// <summary>
        /// List
        /// </summary>
        public static List<Tuple<string, string>> Modules = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>(Module.ARTICLES, GetModuleTranslation(Module.ARTICLES)),
            new Tuple<string, string>(Module.ATTACHMENTS, GetModuleTranslation(Module.ATTACHMENTS)),
            //new Tuple<string, string>(Module.ATTRIBUTES, GetModuleTranslation(Module.ATTRIBUTES)),
            new Tuple<string, string>(Module.ATTRIBUTES_CLASSES, GetModuleTranslation(Module.ATTRIBUTES_CLASSES)),
            //new Tuple<string, string>(Module.COMMUNITY, GetModuleTranslation(Module.COMMUNITY)),
            new Tuple<string, string>(Module.CONTRACTORS, GetModuleTranslation(Module.CONTRACTORS)),
            //new Tuple<string, string>(Module.CONTACTS, GetModuleTranslation(Module.CONTACTS)),
            new Tuple<string, string>(Module.DISTRIBUTIONS, GetModuleTranslation(Module.DISTRIBUTIONS)),
            new Tuple<string, string>(Module.DOCUMENTS, GetModuleTranslation(Module.DOCUMENTS)),
            new Tuple<string, string>(Module.EMPLOYEES, GetModuleTranslation(Module.EMPLOYEES)),
            new Tuple<string, string>(Module.FAMILIES, GetModuleTranslation(Module.FAMILIES)),
            //new Tuple<string, string>(Module.GROUPS, GetModuleTranslation(Module.GROUPS)),
            new Tuple<string, string>(Module.ICONS, GetModuleTranslation(Module.ICONS)),
            new Tuple<string, string>(Module.LOGS, GetModuleTranslation(Module.LOGS)),
            //new Tuple<string, string>(Module.STATS, GetModuleTranslation(Module.STATS)),
            new Tuple<string, string>(Module.STORES, GetModuleTranslation(Module.STORES)),
            new Tuple<string, string>(Module.USERS, GetModuleTranslation(Module.USERS)),
            new Tuple<string, string>(Module.VEHICLES, GetModuleTranslation(Module.VEHICLES)),
        };
    }
}
