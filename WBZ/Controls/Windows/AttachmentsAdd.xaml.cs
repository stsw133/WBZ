using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardText = Clipboard.GetText();
            if (clipboardText.StartsWith("http"))
                tbLink.Text = clipboardText;
            else if (clipboardText.StartsWith("C:"))
                tbDrive.Text = clipboardText;
            tbName.Text = Path.GetFileName(clipboardText);
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
        /// Open internet browser
        /// </summary>
        private void btnLink_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start("");
        }

        /// <summary>
        /// Open file dialog
        /// </summary>
        private void btnDrive_Click(object sender, MouseButtonEventArgs e)
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
        private void btnAccept_Click(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }
}
