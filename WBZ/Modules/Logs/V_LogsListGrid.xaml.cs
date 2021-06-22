﻿using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for LogsListGrid.xaml
    /// </summary>
    public partial class LogsListGrid : DataGrid
    {
        public LogsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_Logs);
		}
    }
}
