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

        /// <summary>
        /// User
        /// </summary>
        public int UserID { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Format
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public byte[] Content { get; set; }
    }
}
