namespace WBZ.Models
{
    public class M_Contact
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Module
        /// </summary>
        public string Module { get; set; } = string.Empty;
        public string TranslatedModule => TranslateMe.TM.Tr(char.ToUpper(Module[0]) + Module[1..], languageId: StswExpress.Globals.Properties.Language);

        /// <summary>
        /// Instance
        /// </summary>
        public int Instance { get; set; } = 0;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Name
        /// </summary>
        public string Forename { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Fullname => $"{Lastname} {Forename}";

        /// <summary>
        /// Default
        /// </summary>
        public bool Default { get; set; } = false;

        /// <summary>
        /// Archival
        /// </summary>
        public bool Archival { get; set; } = false;
    }
}
