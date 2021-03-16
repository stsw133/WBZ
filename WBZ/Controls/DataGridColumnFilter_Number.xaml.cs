﻿using System.Windows;
using System.Windows.Input;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for DataGridColumnFilter_Number.xaml
    /// </summary>
    public partial class DataGridColumnFilter_Number : System.Windows.Controls.StackPanel
    {
        public DataGridColumnFilter_Number()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(pText);
            set { SetValue(pText, value); }
        }
        public static readonly DependencyProperty pText
            = DependencyProperty.Register(
                  nameof(Text),
                  typeof(string),
                  typeof(DataGridColumnFilter_Number),
                  new PropertyMetadata(string.Empty)
              );

        public string Filter
        {
            get => (string)GetValue(pFilter);
            set { SetValue(pFilter, value); }
        }
        public static readonly DependencyProperty pFilter
            = DependencyProperty.Register(
                  nameof(Filter),
                  typeof(string),
                  typeof(DataGridColumnFilter_Number),
                  new PropertyMetadata(string.Empty)
              );

        public bool FilterVisibility
        {
            get => (bool)GetValue(pFilterVisibility);
            set { SetValue(pFilterVisibility, value); }
        }
        public static readonly DependencyProperty pFilterVisibility
            = DependencyProperty.Register(
                  nameof(FilterVisibility),
                  typeof(bool),
                  typeof(DataGridColumnFilter_Number),
                  new PropertyMetadata(true)
              );

        public string Value
        {
            get => (string)GetValue(pValue);
            set { SetValue(pValue, value); }
        }
        public static readonly DependencyProperty pValue
            = DependencyProperty.Register(
                  nameof(Value),
                  typeof(string),
                  typeof(DataGridColumnFilter_Number),
                  new PropertyMetadata(string.Empty)
              );

        /// <summary>
        /// Refresh
        /// </summary>
        private void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dynamic W = Window.GetWindow(this);
            W.cmdRefresh_Executed(sender, e);
        }
    }
}
