using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
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
		private void btnStart_Click(object sender, MouseButtonEventArgs e)
		{
			var dt = new DataTable();

			try
			{
				using (var sqlConn = new NpgsqlConnection(SQL.connWBZ))
				{
					sqlConn.Open();
					using (var sqlTran = sqlConn.BeginTransaction())
					{
						var sqlCmd = new NpgsqlCommand(tbConsole.Text, sqlConn);
						using (var sqlDA = new NpgsqlDataAdapter(sqlCmd))
						{
							sqlDA.Fill(dt);
						}
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
