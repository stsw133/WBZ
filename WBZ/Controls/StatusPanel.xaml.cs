using System.Windows;
using System.Windows.Controls;

namespace WBZ.Controls
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

        public bool HasFilters
        {
            get => (bool)GetValue(pHasFilters);
            set { SetValue(pHasFilters, value); }
        }
        public static readonly DependencyProperty pHasFilters
            = DependencyProperty.Register(
                  nameof(HasFilters),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );

        public bool HasGroups
        {
            get => (bool)GetValue(pHasGroups);
            set { SetValue(pHasGroups, value); }
        }
        public static readonly DependencyProperty pHasGroups
            = DependencyProperty.Register(
                  nameof(HasGroups),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );

        public bool EnableFilters
        {
            get => (bool)GetValue(pEnableFilters);
            set { SetValue(pEnableFilters, value); }
        }
        public static readonly DependencyProperty pEnableFilters
            = DependencyProperty.Register(
                  nameof(EnableFilters),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );
        public bool EnableGroups
        {
            get => (bool)GetValue(pEnableGroups);
            set { SetValue(pEnableGroups, value); }
        }
        public static readonly DependencyProperty pEnableGroups
            = DependencyProperty.Register(
                  nameof(EnableGroups),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );
    }
}
