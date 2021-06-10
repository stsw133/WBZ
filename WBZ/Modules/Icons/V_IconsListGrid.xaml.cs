﻿using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules.Icons
{
    /// <summary>
    /// Interaction logic for IconsListGrid.xaml
    /// </summary>
    public partial class IconsListGrid : DataGrid
    {
        public IconsListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.config_Icons_PanelColor);
		}
    }
}
