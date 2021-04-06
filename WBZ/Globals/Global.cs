using System.Linq;
using System.Security.Cryptography;
﻿using System.Text;
using System.Windows;
using WBZ.Models;

namespace WBZ.Globals
{
    public static class Global
    {
        /// Newest version to download
        public static string VersionNewest { get; set; } = null;
        /// Logged user
        public static M_User User { get; set; } = new M_User();
        /// Module list
        //TODO - przenieść listę modułów do tabeli SQL z kolumnami: ID, Nazwa, Nazwa ikony, Włączony w wersji 1.3.0
        public static class Module
        {
            public const string ARTICLES = "articles";
            public const string ATTACHMENTS = "attachments";
            public const string ATTRIBUTES = "attributes";  // w wersji 1.2.0 przerobienie na submoduł
            public const string ATTRIBUTES_CLASSES = "attributes_classes";
            public const string COMMUNITY = "community";  // do dodania w wersji 1.3.0
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
        public static string GetModuleTranslation(string module) => TranslateMe.TM.Tr(char.ToUpper(module[0]) + module[1..], languageId: StswExpress.Globals.Properties.Language);
        
        public static class UserPermType
        {
            public const string PREVIEW = "preview";
            public const string SAVE = "save";
            public const string DELETE = "delete";
            public const string GROUPS = "groups";
        }

        #region Crypto
        internal static string sha256(string pass)
        {
            var crypt = new SHA256Managed();
            var hash256 = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(pass));
            foreach (byte x in crypto)
                hash256.Append(x.ToString("x2"));

            return hash256.ToString();
        }
        #endregion
    }

    #region Proxy
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get => GetValue(DataProperty);
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
    #endregion
}
