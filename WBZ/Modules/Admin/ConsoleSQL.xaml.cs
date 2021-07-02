using Npgsql;
using System;
using System.Data;
using System.Windows;
using WBZ.Modules._base;

namespace WBZ.Modules.Admin
{
    /// <summary>
    /// Interaction logic for ConsoleSQL.xaml
    /// </summary>
    public partial class ConsoleSQL : Window
    {
        public ConsoleSQL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start
        /// </summary>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var dt = new DataTable();

            try
            {
                using var sqlConn = SQL.ConnOpenedWBZ;
                using var sqlDA = new NpgsqlDataAdapter(TxtBoxConsole.Text, sqlConn);
                sqlDA.Fill(dt);
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd zapytania SQL:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }

            DtgConsole.ItemsSource = dt.DefaultView;
        }
    }
}
