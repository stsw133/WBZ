using StswExpress.Translate;
using System;
using System.Collections.Generic;
using System.Data;

namespace WBZ.Models
{
    /// <summary>
    /// Static model for Documents sources
    /// </summary>
    public static class MS_Documents
    {
        public static List<MV> Statuses { get; } = new List<MV>()
        {
            new MV() { Value = 0, Display = TM.Tr("ToBuffer") },
            new MV() { Value = 1, Display = TM.Tr("Approved") },
            new MV() { Value = 2, Display = TM.Tr("InProgress") },
            new MV() { Value = 3, Display = TM.Tr("Closed") }
        };

        public static List<MV> Types { get; } = new List<MV>()
        {
            new MV() { Value = "fs", Display = "Faktura sprzedaży" }
        };
    }

    /// <summary>
    /// Model for Documents
    /// </summary>
    public class M_Document : M
    {
        /// Name
        public override string Name { get; set; }

        /// Type
        public string Type { get; set; } = (string)MS_Documents.Types[0].Value;

        /// Store
        public int StoreID { get; set; }
        public string StoreName { get; set; }

        /// Contractor
        public int ContractorID { get; set; }
        public string ContractorName { get; set; }

        /// DateIssue
        public DateTime DateIssue { get; set; } = DateTime.Now;

        /// Status
        public int Status { get; set; } = (int)MS_Documents.Statuses[0].Value;

        /// Positions
        public DataTable Positions { get; set; } = new DataTable();
        public int PositionsCount { get; set; }

        /// Weight
        public decimal Weight { get; set; }

        /// Net
        public decimal Net { get; set; }

        /// Tax
        public decimal Tax { get; set; }
    }

    /// <summary>
    /// Model for DocumentsPositions
    /// </summary>
    public class M_DocumentPosition
    {
        /// ID
        public int ID { get; set; }
        public int DocumentID { get; set; }
        public short Pos { get; set; }

        /// Article
        public int ArticleID { get; set; }
        public int ArticleName { get; set; }

        /// Quantity
        public decimal Quantity { get; set; }

        /// Net
        public decimal Net { get; set; }

        /// Tax
        public decimal Tax { get; set; }
    }
}
