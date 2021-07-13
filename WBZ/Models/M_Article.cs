using System.Data;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Articles
    /// </summary>
    public class M_Article : M
    {
        /// Codename
        public string Codename { get; set; }

        /// Name
        public override string Name { get; set; }

        /// EAN
        public string EAN { get; set; }

        /// Measure
        public string Measure { get; set; }

        /// Quantity
        public decimal QuantityRaw { get; set; }
        public decimal Quantity { get; set; }

        /// Reserved
        public decimal ReservedRaw { get; set; }
        public decimal Reserved { get; set; }

        /// Price
        public decimal Price { get; set; }

        /// MainStore
        public int MainStore { get; set; }
        public string MainStoreName { get; set; }

        /// Measures
        public DataTable Measures { get; set; } = new DataTable();
    }

    /// <summary>
    /// Model for ArticlesMeasures
    /// </summary>
    public class M_ArticleMeasure
    {
        /// ID
        public int ID { get; set; }

        /// Name
        public string Name { get; set; }

        /// Converter
        public double Converter { get; set; } = 1;

        /// Price
        public decimal Price { get; set; }

        /// Quantity
        public decimal Quantity { get; set; }

        /// Reserved
        public decimal Reserved { get; set; }

        /// IsDefault
        public bool IsDefault { get; set; }
    }
}
