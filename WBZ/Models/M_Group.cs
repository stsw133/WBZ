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

        /// Name
        public override string Name { get; set; }

        /// Owner
        public int OwnerID { get; set; }

        /// SubGroups
        public List<M_Group> SubGroups { get; set; }

        /// Path
        public string Path { get; set; }
        public string Fullpath => Path + Name;
    }
}
