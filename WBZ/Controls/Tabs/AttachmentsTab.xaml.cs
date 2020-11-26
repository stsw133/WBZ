using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Globals;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for AttachmentsTab.xaml
    /// </summary>
    public partial class AttachmentsTab : UserControl
    {
        D_AttachmentsTab D = new D_AttachmentsTab();
        private string Module;
        private int ID;

        public AttachmentsTab()
        {
            InitializeComponent();
            lbAttachments.DataContext = D;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && D.InstanceAttachments == null)
                {
                    D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Global.Module.ATTACHMENTS, $"a.module='{Module}' and a.instance={ID}");
                    win.Closed += UserControl_Closed;
                }

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                }
            }
            catch { }
        }

        /// <summary>
        /// Add
        /// </summary>
        private void btnAttachmentAdd_Click(object sender, MouseButtonEventArgs e)
        {
            var window = new AttachmentsAdd();
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
                SQL.SetAttachment(Module, ID, window.GetName, file, filePath);
                D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Global.Module.ATTACHMENTS, $"a.module='{Module}' and a.instance={ID}");
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        private void btnAttachmentEdit_Click(object sender, MouseButtonEventArgs e)
        {
            if (lbAttachments.SelectedIndex < 0)
                return;
            var item = lbAttachments.SelectedItem as M_Attachment;

            var window = new MsgWin(MsgWin.Type.InputBox, "Edycja załącznika", "Nowa nazwa załącznika:", item.Name);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                item.Name = window.Value;
                lbAttachments.ItemsSource = D.InstanceAttachments;
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        private void btnAttachmentRemove_Click(object sender, MouseButtonEventArgs e)
        {
            if (lbAttachments.SelectedIndex < 0)
                return;
            var item = lbAttachments.SelectedItem as M_Attachment;

            SQL.DeleteInstance(Global.Module.ATTACHMENTS, item.ID, item.Name);
            D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Global.Module.ATTACHMENTS, $"a.module='{Module}' and a.instance={ID}");
        }

        /// <summary>
        /// SelectionChanged
        /// </summary>
        private void lbAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbAttachments.SelectedIndex < 0)
                return;

            var selection = (sender as ListBox).SelectedItem as M_Attachment;
            if (selection.File == null)
                selection.File = SQL.GetAttachmentFile(selection.ID);

            File.WriteAllBytes(Path.Combine(Path.GetTempPath(), selection.Name), selection.File);

            wbFile.Source = new Uri(@"file:///" + Path.Combine(Path.GetTempPath(), selection.Name));
        }

        /// <summary>
        /// Closed
        /// </summary>
        private void UserControl_Closed(object sender, EventArgs e)
        {
            try
            {
                if (D.InstanceAttachments != null)
                    foreach (var attachment in D.InstanceAttachments)
                        if (attachment.File != null)
                            File.Delete(Path.Combine(Path.GetTempPath(), attachment.Name));
            }
            catch { }
        }
    }

    /// <summary>
    /// Model
    /// </summary>
    internal class D_AttachmentsTab : INotifyPropertyChanged
    {
        /// Attachments
        private List<M_Attachment> instanceAttachments;
        public List<M_Attachment> InstanceAttachments
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