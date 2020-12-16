using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for AttachmentsAdd.xaml
    /// </summary>
    public partial class AttachmentsAdd : Window
    {
        public AttachmentsAdd()
        {
            InitializeComponent();
        }

        public string GetLink
        {
            get
            {
                return tbLink.Text;
            }
        }

        public string GetDrive
        {
            get
            {
                return tbDrive.Text;
            }
        }

        public string GetName
        {
            get
            {
                return tbName.Text;
            }
        }

        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardText = Clipboard.GetText();
            if (clipboardText.StartsWith("http"))
                tbLink.Text = clipboardText;
            else if (clipboardText.StartsWith("C:"))
                tbDrive.Text = clipboardText;
            try
            {
                tbName.Text = Path.GetFileName(clipboardText);
            }
            catch { }
        }

        /// <summary>
        /// Open internet browser
        /// </summary>
        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.google.com/");
        }

        /// <summary>
        /// Open file dialog
        /// </summary>
        private void btnDrive_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Wszystkie pliki|*.*"
            };
            if (dialog.ShowDialog() == true)
                tbDrive.Text = dialog.FileName;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
