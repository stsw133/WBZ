using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WBZ.Modules.Stats
{
	/// <summary>
	/// Interaction logic for Stats.xaml
	/// </summary>
	public partial class Stats : Window
	{
		D_Stats D = new D_Stats();

		public Stats()
		{
			InitializeComponent();
			DataContext = D;
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
				D.StatsArticles = SQL.GetStatsArticles();
				D.StatsArticlesTotal = SQL.GetStatsArticlesTotal();
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
}
