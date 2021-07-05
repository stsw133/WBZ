using System.Collections.Generic;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Groups
    /// </summary>
    public class M_Group : M, IMM
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// <summary>
        /// Owner
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// SubGroups
        /// </summary>
        public List<M_Group> SubGroups { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }
        public string Fullpath => Path + Name;
    }
}
