﻿using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace WBZ.Modules.Stats
{
    class D_Stats : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
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
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
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
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
	}
}
