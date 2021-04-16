using System.Data;
using WBZ.Models;

namespace WBZ.Modules.Stats
{
    class D_Stats : D
    {
		/// Article stats
		private DataTable statsArticles = SQL.GetStatsArticles();
		public DataTable StatsArticles
		{
			get => statsArticles;
			set => SetField(ref statsArticles, value, () => StatsArticles);
		}

		/// Article stats - total
		private double statsArticlesTotal = SQL.GetStatsArticlesTotal();
		public double StatsArticlesTotal
		{
			get => statsArticlesTotal;
			set => SetField(ref statsArticlesTotal, value, () => StatsArticlesTotal);
		}
	}
}
