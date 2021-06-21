using System.Collections.Generic;
using System.Collections.Specialized;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Filters
    /// </summary>
    public class M_Filter : M, IMM
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// ShowArchival
        /// </summary>
        public bool ShowArchival { get; set; }
        
        /// <summary>
        /// ShowGroup
        /// </summary>
        public int ShowGroup { get; set; }

        /// <summary>
        /// ColumnFilterString
        /// </summary>
        public string AutoFilterString { get; set; }

        /// <summary>
        /// ColumnFilterParams
        /// </summary>
        public List<MV> AutoFilterParams { get; set; } = new List<MV>();

        /// <summary>
        /// Sorting
        /// </summary>
        public StringCollection Sorting
        {
            get => Module == null ? new StringCollection() { "2 asc" } : (StringCollection)Properties.Settings.Default[$"sorting_{Module.Name}"];
            set => Properties.Settings.Default[$"sorting_{Module.Name}"] = value;
        }

        /// <summary>
        /// Limit
        /// </summary>
        public int Limit
        {
            get => Module == null ? 50 : (int)Properties.Settings.Default[$"limit_{Module.Name}"];
            set => Properties.Settings.Default[$"limit_{Module.Name}"] = value;
        }
    }
}
