using System;
using System.Collections.Generic;

namespace WBZ.Models
{
    /// <summary>
    /// Static model for Logs sources
    /// </summary>
    public static class MS_Logs
    {
        public static List<MV> Types { get; } = new List<MV>()
        {
            new MV() { Value = 1, Display = "Log" },
            new MV() { Value = 2, Display = "Error" }
        };
    }

    /// <summary>
    /// Model for Logs
    /// </summary>
    public class M_Log : M, IMM
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// Name
        public override string Name { get; set; }

        /// User
        public int UserID { get; set; }
        public string UserName { get; set; }

        /// Type
        public short Type { get; set; } = Convert.ToInt16(MS_Logs.Types[0].Value);

        /// Content
        public string Content { get; set; }

        /// DateCreated
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
