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

        /// <summary>
        /// HasFilters
        /// </summary>
        public bool HasFilters
        {
            get => (bool)GetValue(pHasFilters);
            set => SetValue(pHasFilters, value);
        }
        public static readonly DependencyProperty pHasFilters
            = DependencyProperty.Register(
                  nameof(HasFilters),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );

        /// <summary>
        /// HasGroups
        /// </summary>
        public bool HasGroups
        {
            get => (bool)GetValue(pHasGroups);
            set => SetValue(pHasGroups, value);
        }
        public static readonly DependencyProperty pHasGroups
            = DependencyProperty.Register(
                  nameof(HasGroups),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );
    }
}
