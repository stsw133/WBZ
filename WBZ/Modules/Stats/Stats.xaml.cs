using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Helpers;

namespace WBZ.Modules.Stats
{
	/// <summary>
	/// Interaction logic for Stats.xaml
	/// </summary>
	public partial class Stats : Window
	{
		M_Stats M = new M_Stats();

		public Stats()
		{
			InitializeComponent();
			DataContext = M;
		}

		/// <summary>
		/// Report
		/// </summary>
		private void btnReportGenerate_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new StatsReportsGenerator(StatsReports.DonationsSum);
			window.Owner = this;
			window.ShowDialog();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				M.StatsArticles = SQL.GetStatsArticles();
				M.StatsArticlesTotal = SQL.GetStatsArticlesTotal();
			});
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_Stats : INotifyPropertyChanged
	{
		/// Logged user
		public C_User User { get; } = Global.User;
		/// Article stats
		private DataTable statsArticles = SQL.GetStatsArticles();
		public DataTable StatsArticles
		{
			get
			{
				return statsArticles;
			}
			set
			{
				statsArticles = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Article stats - total
		private double statsArticlesTotal = SQL.GetStatsArticlesTotal();
		public double StatsArticlesTotal
		{
			get
			{
				return statsArticlesTotal;
			}
			set
			{
				statsArticlesTotal = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}

		/// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
