using System.Windows;
using System.Windows.Input;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for DataGridColumnFilter_Text.xaml
    /// </summary>
    public partial class DataGridColumnFilter_Text : System.Windows.Controls.StackPanel
    {
        public DataGridColumnFilter_Text()
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
                  typeof(DataGridColumnFilter_Text),
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
                  typeof(DataGridColumnFilter_Text),
                  new PropertyMetadata(string.Empty)
              );

        public string Value
        {
            get { return (string)GetValue(pValue); }
            set { SetValue(pValue, value); }
        }
        public static readonly DependencyProperty pValue
            = DependencyProperty.Register(
                  nameof(Value),
                  typeof(string),
                  typeof(DataGridColumnFilter_Text),
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
