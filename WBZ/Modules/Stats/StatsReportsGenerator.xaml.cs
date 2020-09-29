using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Other;

namespace WBZ.Modules.Stats
{
    public enum StatsReports
    {
        DonationsSum = 0
    }

    /// <summary>
    /// Interaction logic for StatsReportsGenerator.xaml
    /// </summary>
    public partial class StatsReportsGenerator : Window
    {
        private StatsReports type;

        public StatsReportsGenerator(StatsReports type)
        {
            InitializeComponent();
            this.type = type;
            dpFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpTo.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void btnAccept_Click(object sender, MouseButtonEventArgs e)
        {
            if (dpFrom.SelectedDate.HasValue && dpTo.SelectedDate.HasValue)
            {
                if (dpFrom.SelectedDate.Value > dpTo.SelectedDate.Value)
                {
                    MessageBox.Show("Błędny dobór daty!", "Kontrola składni!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    switch (type)
                    {
                        case StatsReports.DonationsSum: 
                            Prints.Print_ReportGenerateDonationsSum(dpFrom.SelectedDate.Value, dpTo.SelectedDate.Value);
                            break;
                    }
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Ustawienie przedziału czasowego jest niezbędne do wygenerowania raportu!", "Kontrola składni!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
