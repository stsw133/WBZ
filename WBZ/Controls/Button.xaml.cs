using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for Button.xaml
    /// </summary>
    public partial class Button : UserControl
    {
        public Button()
        {
            InitializeComponent();
        }

        public string Alignment
        {
            get { return (string)GetValue(pAlignment); }
            set { SetValue(pAlignment, value); }
        }
        public static readonly DependencyProperty pAlignment
            = DependencyProperty.Register(
                  nameof(Alignment),
                  typeof(string),
                  typeof(Button),
                  new PropertyMetadata("Left")
              );

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(pIcon); }
            set { SetValue(pIcon, value); }
        }
        public static readonly DependencyProperty pIcon
            = DependencyProperty.Register(
                  nameof(Icon),
                  typeof(ImageSource),
                  typeof(Button),
                  new PropertyMetadata(null)
              );

        public string Text
        {
            get { return (string)GetValue(pText); }
            set { SetValue(pText, value); }
        }
        public static readonly DependencyProperty pText
            = DependencyProperty.Register(
                  nameof(Text),
                  typeof(string),
                  typeof(Button),
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
                  typeof(Button),
                  new PropertyMetadata(true)
              );
    }
}
