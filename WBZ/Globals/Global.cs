using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
﻿using System.Text;
using WBZ.Models;
using WBZ.Controls;

namespace WBZ.Globals
{
    public static class Global
    {
        /// App version
        public static string Version => string.Concat(Assembly.GetEntryAssembly().GetName().Version.ToString().Reverse().Skip(2).Reverse());
        /// Newest version to download
        public static string VersionNewest { get; set; } = null;
        /// Chosen database
        public static M_Database Database { get; set; } = new M_Database();
        /// Logged user
        public static M_User User { get; set; } = new M_User();
        /// Module list
        public static class Module
        {
            public const string ARTICLES = "articles";
            public const string ATTACHMENTS = "attachments";
            public const string ATTRIBUTES = "attributes";
            public const string ATTRIBUTES_CLASSES = "attributes_classes";
            public const string COMMUNITY = "community";
            public const string COMPANIES = "companies";
            public const string CONTACTS = "contacts";
            public const string DISTRIBUTIONS = "distributions";
            public const string DOCUMENTS = "documents";
            public const string EMPLOYEES = "employees";
            public const string FAMILIES = "families";
            public const string GROUPS = "groups";
            public const string LOGS = "logs";
            public const string STATS = "stats";
            public const string STORES = "stores";
            public const string USERS = "users";
        }
        public static string TranslateModule(string module)
        {
            switch (module)
            {
                case Module.ARTICLES:
                    return "Towary";
                case Module.ATTACHMENTS:
                    return "Załączniki";
                case Module.ATTRIBUTES_CLASSES:
                    return "Klasy atrybutów";
                case Module.COMMUNITY:
                    return "Społeczność";
                case Module.COMPANIES:
                    return "Firmy";
                case Module.DISTRIBUTIONS:
                    return "Dystrybucje";
                case Module.DOCUMENTS:
                    return "Dokumenty";
                case Module.EMPLOYEES:
                    return "Pracownicy";
                case Module.FAMILIES:
                    return "Rodziny";
                case Module.LOGS:
                    return "Logi";
                case Module.STATS:
                    return "Statystyki";
                case Module.STORES:
                    return "Magazyny";
                case Module.USERS:
                    return "Użytkownicy";
                default:
                    return "(nieznany)";
            }
        }
        public static class UserPermType
        {
            public const string PREVIEW = "preview";
            public const string SAVE = "save";
            public const string DELETE = "delete";
        }
        
		#region Crypto
		private const int DerivationIterations = 1000;
        private const string password = "ejdndbfewbasjhdggjhbasbvdgewvbjdbsavdqgwjbdjsvdyugwqyubashjdbjfgdtyuqw";

        internal static string Encrypt(string plainText)
        {
            if (plainText.Length == 0)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Tekst do zaszyfrowania jest pusty").ShowDialog();
                return "";
            }

            string passPhrase = password;
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(16);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        internal static string Decrypt(string cipherText)
        {
            if (cipherText.Length == 0)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Tekst do odszyfrowania jest pusty").ShowDialog();
                return "";
            }

            /// Get the complete stream of bytes that represent:
            /// [16 bytes of Salt] + [16 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            /// Get the saltbytes by extracting the first 16 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(16).ToArray();
            /// Get the IV bytes by extracting the next 16 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(16).Take(16).ToArray();
            /// Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(16 * 2).Take(cipherTextBytesWithSaltAndIv.Length - (16 * 2)).ToArray();
            string passPhrase = password;
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(16);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; /// 16 Bytes will give us 128 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                /// Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

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
}
