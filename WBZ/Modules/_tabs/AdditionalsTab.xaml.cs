using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Controls;

namespace WBZ.Modules._tabs
{
    /// <summary>
    /// Interaction logic for AdditionalsTab.xaml
    /// </summary>
    public partial class AdditionalsTab : UserControl
    {
        public AdditionalsTab()
        {
            InitializeComponent();
        }

        public bool HasIcon
        {
            get => (bool)GetValue(pHasIcon);
            set { SetValue(pHasIcon, value); }
        }
        public static readonly DependencyProperty pHasIcon
            = DependencyProperty.Register(
                  nameof(HasIcon),
                  typeof(bool),
                  typeof(GroupsView),
                  new PropertyMetadata(true)
              );

        /// <summary>
        /// ManageIcon - drop image
        /// </summary>
        private void btnManageIcon_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                byte[] file = File.ReadAllBytes(files[0]);

                Window win = Window.GetWindow(this);
                dynamic d = win?.DataContext;
                if (d != null)
                {
                    d.InstanceInfo.Icon = file;
                    d.InstanceInfo = d.InstanceInfo;
                }
            }
        }

        /// <summary>
		/// ManageIcon - open context menu
		/// </summary>
		private void btnManageIcon_Click(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn != null)
                btn.ContextMenu.IsOpen = true;
        }

        /// <summary>
		/// ManageIcon - Load
		/// </summary>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
		{
            var window = new AttachmentsAdd(false);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                string filePath;
                byte[] file;
                if (string.IsNullOrEmpty(window.GetDrive))
                {
                    filePath = window.GetLink;
                    using (WebClient client = new WebClient())
                    {
                        file = client.DownloadData(filePath);
                    }
                }
                else
                {
                    filePath = window.GetDrive;
                    file = File.ReadAllBytes(filePath);
                }

                Window win = Window.GetWindow(this);
                dynamic d = win?.DataContext;
                if (d != null)
                {
                    d.InstanceInfo.Icon = file;
                    d.InstanceInfo = d.InstanceInfo;
                }
            }
        }

        /// <summary>
		/// ManageIcon - Delete
		/// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            if (d != null)
            {
                d.InstanceInfo.Icon = null;
                d.InstanceInfo = d.InstanceInfo;
            }
        }
    }
}