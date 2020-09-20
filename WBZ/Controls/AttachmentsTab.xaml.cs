using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for AttachmentsTab.xaml
    /// </summary>
    public partial class AttachmentsTab : UserControl
    {
        M_AttachmentsTab M = new M_AttachmentsTab();
        private string InstanceType;
        private int ID;
        private bool EditMode;

        public AttachmentsTab()
        {
            InitializeComponent();
            DataContext = M;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && M.InstanceAttachments == null)
                {
                    M.InstanceAttachments = SQL.ListAttachments(InstanceType, ID);
                    win.Closed += UserControl_Closed;
                }

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    InstanceType = (string)d.INSTANCE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                    EditMode = (bool)d.EditMode;
                }
            }
            catch { }
        }

        private void btnAttachmentAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Wszystkie pliki|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                SQL.SetAttachment(InstanceType, ID, Path.GetFileName(dialog.FileName), File.ReadAllBytes(dialog.FileName));
                M.InstanceAttachments = SQL.ListAttachments(InstanceType, ID);
            }
        }
        private void btnAttachmentRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbAttachments.SelectedIndex < 0)
                return;

            SQL.DeleteAttachment(M.InstanceAttachments[lbAttachments.SelectedIndex].ID);
            SQL.SetLog(Global.User.ID, InstanceType, ID, $"Usunięto załącznik: {M.InstanceAttachments[lbAttachments.SelectedIndex].Name}");

            M.InstanceAttachments = SQL.ListAttachments(InstanceType, ID);
        }
        private void lbAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbAttachments.SelectedIndex < 0)
                return;

            var selection = (sender as ListBox).SelectedItem as C_Attachment;
            if (selection.File == null)
                selection.File = SQL.GetAttachmentFile(selection.ID);

            File.WriteAllBytes(Path.Combine(Path.GetTempPath(), selection.Name), selection.File);

            wbFile.Source = new Uri(@"file:///" + Path.Combine(Path.GetTempPath(), selection.Name));
        }

        private void UserControl_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();

            try
            {
                if (M.InstanceAttachments != null)
                    foreach (var attachment in M.InstanceAttachments)
                        if (attachment.File != null)
                            File.Delete(Path.Combine(Path.GetTempPath(), attachment.Name));
            }
            catch { }
        }
    }

    /// <summary>
    /// Model
    /// </summary>
    internal class M_AttachmentsTab : INotifyPropertyChanged
    {
        /// Załączniki
        private List<C_Attachment> instanceAttachments;
        public List<C_Attachment> InstanceAttachments
        {
            get
            {
                return instanceAttachments;
            }
            set
            {
                instanceAttachments = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}