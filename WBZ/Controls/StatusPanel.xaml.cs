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

        public bool EnableGroups
        {
            get { return (bool)GetValue(pEnableGroups); }
            set { SetValue(pEnableGroups, value); }
        }
        public static readonly DependencyProperty pEnableGroups
            = DependencyProperty.Register(
                  nameof(EnableGroups),
                  typeof(bool),
                  typeof(StatusPanel),
                  new PropertyMetadata(false)
              );

        public bool HasGroups
        {
            get { return (bool)GetValue(pHasGroups); }
            set { SetValue(pHasGroups, value); }
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
