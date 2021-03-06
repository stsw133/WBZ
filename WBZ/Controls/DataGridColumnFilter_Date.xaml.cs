﻿using System.Windows;
using System.Windows.Input;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for DataGridColumnFilter_Date.xaml
    /// </summary>
    public partial class DataGridColumnFilter_Date : System.Windows.Controls.StackPanel
    {
        public DataGridColumnFilter_Date()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(pText); }
            set { SetValue(pText, value); }
        }
        public static readonly DependencyProperty pText
            = DependencyProperty.Register(
                  nameof(Text),
                  typeof(string),
                  typeof(DataGridColumnFilter_Date),
                  new PropertyMetadata(string.Empty)
              );

        public string Filter
        {
            get { return (string)GetValue(pFilter); }
            set { SetValue(pFilter, value); }
        }
        public static readonly DependencyProperty pFilter
            = DependencyProperty.Register(
                  nameof(Filter),
                  typeof(string),
                  typeof(DataGridColumnFilter_Date),
                  new PropertyMetadata(string.Empty)
              );

        public bool FilterVisibility
        {
            get { return (bool)GetValue(pFilterVisibility); }
            set { SetValue(pFilterVisibility, value); }
        }
        public static readonly DependencyProperty pFilterVisibility
            = DependencyProperty.Register(
                  nameof(FilterVisibility),
                  typeof(bool),
                  typeof(DataGridColumnFilter_Date),
                  new PropertyMetadata(true)
              );

        public string Value1
        {
            get { return (string)GetValue(pValue1); }
            set { SetValue(pValue1, value); }
        }
        public static readonly DependencyProperty pValue1
            = DependencyProperty.Register(
                  nameof(Value1),
                  typeof(string),
                  typeof(DataGridColumnFilter_Date),
                  new PropertyMetadata(string.Empty)
              );

        public string Value2
        {
            get { return (string)GetValue(pValue2); }
            set { SetValue(pValue2, value); }
        }
        public static readonly DependencyProperty pValue2
            = DependencyProperty.Register(
                  nameof(Value2),
                  typeof(string),
                  typeof(DataGridColumnFilter_Date),
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
