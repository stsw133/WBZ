using StswExpress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for AttachmentsTab.xaml
    /// </summary>
    public partial class AttachmentsTab : UserControl
    {
        readonly D_AttachmentsTab D = new D_AttachmentsTab();

        public AttachmentsTab()
        {
            InitializeComponent();
            //DataContext = D;
        }

        /// <summary>
        /// Module
        /// </summary>
        public static readonly DependencyProperty ModuleProperty
            = DependencyProperty.Register(
                  nameof(Module),
                  typeof(MV),
                  typeof(AttachmentsTab),
                  new PropertyMetadata(default(MV))
              );
        public MV Module
        {
            get => (MV)GetValue(ModuleProperty);
            set => SetValue(ModuleProperty, value);
        }

        /// <summary>
        /// InstanceID
        /// </summary>
        public static readonly DependencyProperty InstanceIDProperty
            = DependencyProperty.Register(
                  nameof(InstanceID),
                  typeof(int),
                  typeof(AttachmentsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InstanceID > 0 && D.InstanceAttachments == null)
                    D.InstanceAttachments = SQL.ListInstances<M_Attachment>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
                DataContext = D;
                Loaded -= UserControl_Loaded;
                Window.GetWindow(this).Closed += UserControl_Closed;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd inicjalizacji zakładki załączników", ex, Module, InstanceID);
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        private void BtnAttachmentAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AttachmentsAdd(true);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                string filePath;
                byte[] file;

                if (string.IsNullOrEmpty(window.GetDrive))
                {
                    filePath = window.GetLink;
                    using (var client = new WebClient())
                    {
                        file = client.DownloadData(filePath);
                    }
                }
                else
                {
                    filePath = window.GetDrive;
                    file = File.ReadAllBytes(filePath);
                }

                if (file.Length > Convert.ToInt32(Config.Attachment_Size_Max))
                {
                    new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Załącznik przekracza dopuszczalny rozmiar: " + Config.Attachment_Size_Max) { Owner = Window.GetWindow(this) }.ShowDialog();
                    return;
                }

                SQL.SetAttachment(Module, InstanceID, window.GetName, filePath, file, filePath);
                D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Config.GetModule(nameof(Attachments)), $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        private void BtnAttachmentEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LstBoxAttachments.SelectedIndex < 0)
                return;
            var item = LstBoxAttachments.SelectedItem as M_Attachment;

            var window = new MsgWin(MsgWin.Types.InputBox, "Edycja załącznika", "Nowa nazwa załącznika:", item.Name);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                item.Name = window.Value;
                LstBoxAttachments.ItemsSource = D.InstanceAttachments;
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        private void BtnAttachmentRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LstBoxAttachments.SelectedIndex < 0)
                return;
            var item = LstBoxAttachments.SelectedItem as M_Attachment;

            SQL.DeleteInstance(Config.GetModule(nameof(Attachments)), item.ID, item.Name);
            D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Config.GetModule(nameof(Attachments)), $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
        }

        /// <summary>
        /// Drop
        /// </summary>
        private void LstBoxAttachments_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = File.ReadAllBytes(files[0]);

                SQL.SetAttachment(Module, InstanceID, Path.GetFileName(files[0]), files[0], file, files[0]);
                D.InstanceAttachments = SQL.ListInstances<M_Attachment>(Config.GetModule(nameof(Attachments)), $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
            }
        }

        /// <summary>
        /// SelectionChanged
        /// </summary>
        private void LstBoxAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstBoxAttachments.SelectedIndex < 0)
                return;

            var selection = (sender as ListBox).SelectedItem as M_Attachment;
            if (selection.Content == null)
                selection.Content = SQL.GetAttachmentFile(selection.ID);

            File.WriteAllBytes(Path.Combine(Path.GetTempPath(), selection.Name), selection.Content);
            WebBwsFile.Source = new Uri(@"file:///" + Path.Combine(Path.GetTempPath(), selection.Name));
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
                        if (attachment.Content != null)
                            File.Delete(Path.Combine(Path.GetTempPath(), attachment.Name));
            }
            catch { }
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    internal class D_AttachmentsTab : D
    {
        /// Module
        public MV Module = Config.GetModule(nameof(Attachments));

        /// Attachments
        private List<M_Attachment> instanceAttachments;
        public List<M_Attachment> InstanceAttachments
        {
            get => instanceAttachments;
            set => SetField(ref instanceAttachments, value, () => InstanceAttachments);
        }
    }
}