using System.Collections.Generic;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Users
    /// </summary>
    public class M_User : M, IMP
    {
        /// IMP
        public override string Name => $"{Lastname} {Forename}";
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        /// <summary>
        /// Login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Newpass
        /// </summary>
        public string Newpass { get; set; }

        /// <summary>
        /// IsBlocked
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Perms
        /// </summary>
        public List<string> Perms { get; set; } = new List<string>();
    }
}
