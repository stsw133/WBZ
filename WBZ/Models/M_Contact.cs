using WBZ.Globals;

namespace WBZ.Models
{
    public class M_Contact
    {
        public int ID { get; set; }
        public string Module { get; set; }
        public int Instance { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Forename { get; set; }
        public string Lastname { get; set; }
        public bool Default { get; set; }
        public bool Archival { get; set; }

        public M_Contact()
        {
            Module = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Forename = string.Empty;
            Lastname = string.Empty;
        }

        public string TranslatedModule
        {
            get
            {
                return Global.TranslateModule(Module);
            }
        }
    }
}
