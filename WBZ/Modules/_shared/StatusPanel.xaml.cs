using System.Windows;
using System.Windows.Controls;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel : UserControl
    {
        public StatusPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// HasFilters
        /// </summary>
        public static readonly DependencyProperty HasFiltersProperty
            = DependencyProperty.Register(
                  nameof(HasFilters),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );
        public bool HasFilters
        {
            get => (bool)GetValue(HasFiltersProperty);
            set => SetValue(HasFiltersProperty, value);
        }

        /// <summary>
        /// HasGroups
        /// </summary>
        public static readonly DependencyProperty HasGroupsProperty
            = DependencyProperty.Register(
                  nameof(HasGroups),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );
        public bool HasGroups
        {
            get => (bool)GetValue(HasGroupsProperty);
            set => SetValue(HasGroupsProperty, value);
        }
    }
}
