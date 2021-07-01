using System.Windows;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for AppAbout.xaml
    /// </summary>
    public partial class AppAbout : Window
    {
        readonly D_AppAbout D = new D_AppAbout();

        public AppAbout()
        {
            InitializeComponent();
            DataContext = D;
        }
    }
}
