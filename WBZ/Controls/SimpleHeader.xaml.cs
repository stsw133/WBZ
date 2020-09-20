using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for SimpleHeader.xaml
    /// </summary>
    public partial class SimpleHeader : UserControl
	{
		public SimpleHeader()
		{
			InitializeComponent();
		}

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(pIconSource); }
            set { SetValue(pIconSource, value); }
        }

        public static readonly DependencyProperty pIconSource
            = DependencyProperty.Register(
                  nameof(IconSource),
                  typeof(ImageSource),
                  typeof(SimpleHeader),
                  new PropertyMetadata(null)
              );

        public string TextContent
        {
            get { return (string)GetValue(pTextContent); }
            set { SetValue(pTextContent, value); }
        }

        public static readonly DependencyProperty pTextContent
            = DependencyProperty.Register(
                  nameof(TextContent),
                  typeof(string),
                  typeof(SimpleHeader),
                  new PropertyMetadata("")
              );
    }
}
