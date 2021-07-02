using System.Data;

namespace WBZ.Models
{
    /// <summary>
    /// Model for Articles
    /// </summary>
    public class M_Article : M
    {
        /// <summary>
        /// Codename
        /// </summary>
        public string Codename { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// EAN
        /// </summary>
        public string EAN { get; set; }

        /// <summary>
        /// Measure
        /// </summary>
        public string Measure { get; set; }

        /// <summary>
        /// AmountRaw
        /// </summary>
        public decimal QuantityRaw { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// ReservedRaw
        /// </summary>
        public decimal ReservedRaw { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public decimal Reserved { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// MainStore
        /// </summary>
        public int MainStore { get; set; }
        public string MainStoreName { get; set; }

        /// <summary>
        /// Measures
        /// </summary>
        public DataTable Measures { get; set; } = new DataTable();
    }

    /// <summary>
    /// Model for ArticlesMeasures
    /// </summary>
    public class M_ArticleMeasure
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Converter
        /// </summary>
        public double Converter { get; set; } = 1;

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public decimal Reserved { get; set; }

        /// <summary>
        /// IsDefault
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
