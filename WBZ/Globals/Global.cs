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

        /// <summary>
        /// UserPermType
        /// </summary>
        public static class PermType
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
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
    #endregion
}
