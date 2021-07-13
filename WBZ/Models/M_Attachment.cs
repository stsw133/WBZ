namespace WBZ.Models
{
    /// <summary>
    /// Model for Attachments
    /// </summary>
    public class M_Attachment : M, IMM
    {
        /// IMM
        public MV Module { get; set; }
        public int InstanceID { get; set; }

        /// Name
        public override string Name { get; set; }

        /// User
        public int UserID { get; set; }
        public string UserName { get; set; }

        /// Path
        public string Path { get; set; }
        public string Format { get; set; }

        /// Size
        public double Size { get; set; }

        /// Content
        public byte[] Content { get; set; }
    }
}
