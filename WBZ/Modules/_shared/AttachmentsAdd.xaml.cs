using Microsoft.Win32;
using StswExpress;
using System.Linq;
using System.Windows;
using WBZ.Modules._base;

namespace WBZ.Modules._shared
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
                DckPanName.Visibility = Visibility.Collapsed;
        }

        public string GetLink => TxtBoxLink.Text;
        public string GetDrive => TxtBoxDrive.Text;
        public string GetName => TxtBoxName.Text;

        /// <summary>
        /// Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardText = Clipboard.GetText();
            if (clipboardText.StartsWith("http"))
                TxtBoxLink.Text = clipboardText;
            else if (clipboardText.StartsWith("C:"))
                TxtBoxDrive.Text = clipboardText;

            if (!string.IsNullOrEmpty(TxtBoxLink.Text))
                TxtBoxName.Text = TxtBoxLink.Text.Split('\\').Last().Split('.').First();
            else if (!string.IsNullOrEmpty(TxtBoxDrive.Text))
                TxtBoxName.Text = TxtBoxDrive.Text.Split('/').Last().Split('.').First();
        }

        /// <summary>
        /// Open internet browser
        /// </summary>
        private void BtnLink_Click(object sender, RoutedEventArgs e)
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
        private void BtnDrive_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "Wszystkie pliki|*.*" };
            if (dialog.ShowDialog() == true)
                TxtBoxDrive.Text = dialog.FileName;
        }

        /// <summary>
        /// Accept
        /// </summary>
        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (needName && string.IsNullOrWhiteSpace(GetName))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Należy uzupełnić nazwę załącznika!") { Owner = this }.ShowDialog();
                return;
            }
            DialogResult = true;
        }
    }
}
