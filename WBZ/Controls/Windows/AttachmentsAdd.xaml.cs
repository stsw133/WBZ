using Microsoft.Win32;
using StswExpress;
using System.Linq;
using System.Windows;
using WBZ.Modules._base;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for AttachmentsAdd.xaml
    /// </summary>
    public partial class AttachmentsAdd : Window
    {
        bool needName;

        public AttachmentsAdd(bool needName = true)
        {
            InitializeComponent();

            this.needName = needName;
            if (!needName)
                dpName.Visibility = Visibility.Collapsed;
        }

        public string GetLink=> tbLink.Text;
        public string GetDrive => tbDrive.Text;
        public string GetName => tbName.Text;

        /// <summary>
        /// Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardText = Clipboard.GetText();
            if (clipboardText.StartsWith("http"))
                tbLink.Text = clipboardText;
            else if (clipboardText.StartsWith("C:"))
                tbDrive.Text = clipboardText;

            if (!string.IsNullOrEmpty(tbLink.Text))
                tbName.Text = tbLink.Text.Split('\\').Last().Split('.').First();
            else if (!string.IsNullOrEmpty(tbDrive.Text))
                tbName.Text = tbDrive.Text.Split('/').Last().Split('.').First();
        }

        /// <summary>
        /// Open internet browser
        /// </summary>
        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Fn.OpenFile("https://www.google.com/");
            }
            catch { }
        }

        /// <summary>
        /// Open file dialog
        /// </summary>
        private void btnDrive_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "Wszystkie pliki|*.*" };
            if (dialog.ShowDialog() == true)
                tbDrive.Text = dialog.FileName;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (needName && string.IsNullOrWhiteSpace(GetName))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Należy uzupełnić nazwę załącznika!") { Owner = this }.ShowDialog();
                return;
            }
            DialogResult = true;
        }
    }
}
