using Npgsql;
using System;
using System.Data;
using System.Windows;
using WBZ.Controls;

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
		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			var dt = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(SQL.connWBZ))
				{
					sqlConn.Open();

					var sqlCmd = new NpgsqlCommand(tbConsole.Text, sqlConn);
					using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
					{
						sqlDA.Fill(dt);
					}
				}
			}
			catch (Exception ex)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd zapytania SQL: " + ex.Message) { Owner = this }.ShowDialog();
			}

			dgConsole.ItemsSource = dt.DefaultView;
		}
	}
}
