﻿using System.Text;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.IO;
using System.Windows;
using WBZ.Classes;
using System.Reflection;

namespace WBZ.Helpers
{
    internal static class Global
    {
        internal static string Version => $"{Assembly.GetEntryAssembly().GetName().Version.Major}.{Assembly.GetEntryAssembly().GetName().Version.Minor}.{Assembly.GetEntryAssembly().GetName().Version.Build}";
        internal static string VersionNewest { get; set; } = null;
        internal static C_Database Database { get; set; } = new C_Database();
        internal static C_User User { get; set; } = new C_User();

        #region GlobalStructure
        internal static class ModuleTypes
        {
            public const string ARTICLES = "articles";
            public const string ATTACHMENTS = "attachments";
            public const string ATTRIBUTES_CLASSES = "attributes_classes";
            public const string COMMUNITY = "community";
            public const string COMPANIES = "companies";
            public const string DISTRIBUTIONS = "distributions";
            public const string DOCUMENTS = "documents";
            public const string EMPLOYEES = "employees";
            public const string FAMILIES = "families";
            public const string LOGS = "logs";
            public const string STATS = "stats";
            public const string STORES = "stores";
            public const string USERS = "users";
        }
        internal static class UserPermTypes
        {
            public const string PREVIEW = "preview";
            public const string SAVE = "save";
            public const string DELETE = "delete";
        }
        #endregion

        #region Translations
        internal static string TranslateModules(string module)
        {
            switch(module)
            {
                case ModuleTypes.ARTICLES:
                    return "Towary";
                case ModuleTypes.ATTACHMENTS:
                    return "Załączniki";
                case ModuleTypes.ATTRIBUTES_CLASSES:
                    return "Klasy atrybutów";
                case ModuleTypes.COMMUNITY:
                    return "Społeczność";
                case ModuleTypes.COMPANIES:
                    return "Firmy";
                case ModuleTypes.DISTRIBUTIONS:
                    return "Dystrybucje";
                case ModuleTypes.DOCUMENTS:
                    return "Dokumenty";
                case ModuleTypes.EMPLOYEES:
                    return "Pracownicy";
                case ModuleTypes.FAMILIES:
                    return "Rodziny";
                case ModuleTypes.LOGS:
                    return "Logi";
                case ModuleTypes.STATS:
                    return "Statystyki";
                case ModuleTypes.STORES:
                    return "Magazyny";
                case ModuleTypes.USERS:
                    return "Użytkownicy";
                default:
                    return "(nieznany)";
            }
        }
		#endregion

		#region Crypto
		private const int DerivationIterations = 1000;
        private const string password = "ejdndbfewbasjhdggjhbasbvdgewvbjdbsavdqgwjbdjsvdyugwqyubashjdbjfgdtyuqw";

        public static string Encrypt(string plainText)
        {
            if (plainText.Length == 0) {
                MessageBox.Show("Tekst do zaszyfrowania jest pusty", "Błąd walidacji!", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public static string Decrypt(string cipherText)
        {
            if (cipherText.Length == 0)
            {
                MessageBox.Show("Tekst do odszyfrowania jest pusty", "Błąd walidacji!", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            // Get the complete stream of bytes that represent:
            // [16 bytes of Salt] + [16 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 16 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(16).ToArray();
            // Get the IV bytes by extracting the next 16 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(16).Take(16).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
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
            var randomBytes = new byte[16]; // 16 Bytes will give us 128 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
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
