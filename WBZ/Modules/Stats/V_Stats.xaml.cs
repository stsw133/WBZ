using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WBZ.Globals;

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
		private void btnReportGenerate_Click(object sender, RoutedEventArgs e)
		{
			var window = new StatsReportsGenerator(StatsReports.DonationsSum);
			window.Owner = this;
			window.ShowDialog();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await Task.Run(() => {
				D.StatsArticles = SQL.GetStatsArticles();
				D.StatsArticlesTotal = SQL.GetStatsArticlesTotal();
			});
		}

		/// <summary>
		/// Close
		/// </summary>
		private void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
