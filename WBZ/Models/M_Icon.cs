namespace WBZ.Models
{
    /// <summary>
    /// Model for Icons
    /// </summary>
    public class M_Icon : M, IMM
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// Name
        public override string Name { get; set; }

        /// Path
        public string Path { get; set; }
        public string Format { get; set; }

        /// Dimensions
        public int Height { get; set; }
        public int Width { get; set; }

        /// Size
        public double Size { get; set; }

        /// Content
        public byte[] Content { get; set; }
    }
}
