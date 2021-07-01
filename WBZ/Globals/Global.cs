using System.Security.Cryptography;
﻿using System.Text;
using System.Windows;

namespace WBZ.Globals
{
    public static class Global  //TODO - docelowo klasa do usunięcia
    {
        #region Crypto
        internal static string sha256(string pass)
        {
            var crypt = new SHA256Managed();
            var hash256 = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(pass));
            foreach (var x in crypto)
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
