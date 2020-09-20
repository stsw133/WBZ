using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for SimpleButton.xaml
    /// </summary>
    public partial class SimpleButton : UserControl
    {
        public SimpleButton()
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
                  typeof(SimpleButton),
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
                  typeof(SimpleButton),
                  new PropertyMetadata("")
              );

        public bool TextVisibility
        {
            get { return (bool)GetValue(pTextVisibility); }
            set { SetValue(pTextVisibility, value); }
        }

        public static readonly DependencyProperty pTextVisibility
            = DependencyProperty.Register(
                  nameof(TextVisibility),
                  typeof(bool),
                  typeof(SimpleButton),
                  new PropertyMetadata(true)
              );
    }
}
