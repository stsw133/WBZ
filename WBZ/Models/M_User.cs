using System.Collections.Generic;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Users
    /// </summary>
    public class M_User : M, IMP
    {
        /// IMP
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        /// Name
        public override string Name => $"{Lastname} {Forename}";

        /// Login
        public string Login { get; set; }

        /// Newpass
        public string Newpass { get; set; }

        /// IsBlocked
        public bool IsBlocked { get; set; }

        /// Perms
        public List<string> Perms { get; set; } = new List<string>();
    }
}
