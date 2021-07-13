using System.Collections.Generic;
using System.Collections.Specialized;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Filters
    /// </summary>
    public class M_Filter : M, IMM
    {
        public M_Filter(MV module)
        {
            Module = module;
        }

        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// Name
        public override string Name { get; set; }

        /// Content
        public string Content { get; set; }

        /// Show...
        public bool ShowArchival { get; set; }
        public int ShowGroup { get; set; }

        /// ColumnFilter...
        public string AutoFilterString { get; set; }
        public List<MV> AutoFilterParams { get; set; } = new List<MV>();

        /// Sorting & limit
        public StringCollection Sorting
        {
            get => Module == null ? new StringCollection() { "2 asc" } : (StringCollection)Properties.Settings.Default[$"sorting_{Module.Name}"];
            set => Properties.Settings.Default[$"sorting_{Module.Name}"] = value;
        }
        public int Limit
        {
            get => Module == null ? 50 : (int)Properties.Settings.Default[$"limit_{Module.Name}"];
            set => Properties.Settings.Default[$"limit_{Module.Name}"] = value;
        }
    }
}
