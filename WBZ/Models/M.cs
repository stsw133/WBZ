namespace WBZ.Models
{
    /// <summary>
    /// Main (base) model interface
    /// </summary>
    public class M
    {
        public int ID { get; set; }
        public virtual string Name { get; set; }

        public string Comment { get; set; }
        public int GroupID { get; set; }
        public int IconID { get; set; }
        public byte[] IconContent { get; set; }
        public bool IsArchival { get; set; }
    }

    /// <summary>
    /// Model interface with address data
    /// </summary>
    public interface IMA
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
    }

    /// <summary>
    /// Model interface with another module reference
    /// </summary>
    public interface IMM
    {
        public MV Module { get; set; }
        public int InstanceID { get; set; }
    }

    /// <summary>
    /// Model interface with personal data
    /// </summary>
    public interface IMP
    {
        public string Forename { get; set; }
        public string Lastname { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
    }

    /// <summary>
    /// Helper model for simple data like values to ComboBoxes
    /// </summary>
    public class MV
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public object Value { get; set; }
        public object Display { get; set; }
        public object Tag { get; set; }
    }
}
