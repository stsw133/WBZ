using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private void btnLink_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start("");
        }

        private void btnDrive_Click(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Wszystkie pliki|*.*"
            };
            if (dialog.ShowDialog() == true)
                tbDrive.Text = dialog.FileName;
        }

        private void btnAccept_Click(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }
}
