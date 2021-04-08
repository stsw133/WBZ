using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace WBZ.Modules.Stats
{
    class D_Stats : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Article stats
		private DataTable statsArticles = SQL.GetStatsArticles();
		public DataTable StatsArticles
		{
			get => statsArticles;
			set
			{
				statsArticles = value;
				NotifyPropertyChanged();
			}
		}
		/// Article stats - total
		private double statsArticlesTotal = SQL.GetStatsArticlesTotal();
		public double StatsArticlesTotal
		{
			get => statsArticlesTotal;
			set
			{
				statsArticlesTotal = value;
				NotifyPropertyChanged();
			}
		}
	}
}
