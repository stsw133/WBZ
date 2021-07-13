using StswExpress.Translate;
using System;
using System.Collections.Generic;
using System.Data;
using WBZ.Globals;

namespace WBZ.Models
{
    /// <summary>
    /// Static model for Distributions sources
    /// </summary>
    public static class MS_Distributions
    {
        public static List<MV> Statuses { get; } = new List<MV>()
        {
            new MV() { Value = 0, Display = TM.Tr("ToBuffer") },
            new MV() { Value = 1, Display = TM.Tr("Approved") },
            new MV() { Value = 2, Display = TM.Tr("InProgress") },
            new MV() { Value = 3, Display = TM.Tr("Closed") }
        };
    }

    /// <summary>
    /// Model for Distributions
    /// </summary>
    public class M_Distribution : M
    {
        /// Name
        public override string Name { get; set; }

        /// DateReal
        public DateTime DateReal { get; set; } = DateTime.Now;

        /// Status
        public int Status { get; set; } = (int)MS_Distributions.Statuses[0].Value;

        /// Families
        public List<M_DistributionFamily> Families { get; set; } = new List<M_DistributionFamily>();
        public int FamiliesCount { get; set; }

        /// MembersCount
        public int MembersCount { get; set; }

        /// PositionsCount
        public int PositionsCount { get; set; }

        /// Weight
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Model for DistributionsFamilies
    /// </summary>
    public class M_DistributionFamily
    {
        public enum DistributionFamilyStatus
        {
            None = 0,
            Informed = 1,
            Taken = 2
        }

        /// Family
        public int FamilyID { get; set; }
        public string FamilyName { get; set; }

        /// Members
        public short? Members { get; set; }

        /// Status
        public int Status { get; set; } = (int)DistributionFamilyStatus.None;

        /// Positions
        public DataTable Positions { get; set; } = SQL.GetDistributionPositionsFormatting();
    }

    /// <summary>
    /// Model for DistributionsPositions
    /// </summary>
    public class M_DistributionPosition
    {
        /// ID
        public int ID { get; set; }
        public int DistributionID { get; set; }
        public short Pos { get; set; }

        /// Store
        public int StoreID { get; set; }
        public string StoreName { get; set; }

        /// Article
        public int ArticleID { get; set; }
        public string ArticleName { get; set; }

        /// Quantity
        public decimal Quantity { get; set; }

        public M_DistributionPosition()
        {
            var stores = SQL.ComboSource(Config.GetModule(nameof(Modules.Stores)), "codename", "true", false);
            if (stores.Count > 0 && StoreID == 0)
            {
                StoreID = (int)stores[0].Value;
                StoreName = (string)stores[0].Display;
            }
        }
    }
}
