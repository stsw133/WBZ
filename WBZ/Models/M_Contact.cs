namespace WBZ.Models
{
    public class M_Contact
    {
        public int ID { get; set; } = 0;
        public string Module { get; set; } = string.Empty;
        public int Instance { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Forename { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public bool Archival { get; set; } = false;

        public string TranslatedModule { get => TranslateMe.TM.Tr(char.ToUpper(Module[0]) + Module[1..], languageId: StswExpress.Globals.Properties.Language); }
    }
}
