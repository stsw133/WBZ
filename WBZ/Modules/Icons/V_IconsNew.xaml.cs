using Microsoft.Win32;
using StswExpress;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Icon;

namespace WBZ.Modules.Icons
{
	/// <summary>
	/// Interaction logic for IconsNew.xaml
	/// </summary>
	public partial class IconsNew : New
	{
		readonly D_IconsNew D = new D_IconsNew();

        public IconsNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceData = instance;
            D.Mode = mode;
        }

        /// <summary>
        /// GetHyperLink
        /// </summary>
        private void btnGetHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Fn.OpenFile("https://www.google.com/");
            }
            catch { }
        }

        /// <summary>
        /// GetFile
        /// </summary>
        private void btnGetFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "Wszystkie pliki|*.*" };
            if (dialog.ShowDialog() == true)
            {
                D.InstanceData.Path = dialog.FileName;
                D.NotifyPropertyChanged("InstanceData");
            }
        }

        /// <summary>
        /// TextChanged
        /// </summary>
        private async void tbPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            D.InstanceData.Path = (sender as TextBox).Text;
            await Task.Run(() =>
            {
                try
                {
                    if (D.InstanceData.Path.StartsWith("http"))
                    {
                        using (var client = new WebClient())
                        {
                            D.InstanceData.File = client.DownloadData(D.InstanceData.Path);
                        }
                    }
                    else
                        D.InstanceData.File = File.ReadAllBytes(D.InstanceData.Path);

                    D.InstanceData.Format = D.InstanceData.Path.Split('.').Last();
                    if (string.IsNullOrEmpty(D.InstanceData.Name))
                        D.InstanceData.Name = Path.GetFileName(D.InstanceData.Path).TrimEnd(D.InstanceData.Format.ToCharArray());

                    var image = Fn.LoadImage(D.InstanceData.File);
                    
                    D.InstanceData.Height = image.PixelHeight;
                    D.InstanceData.Width = image.PixelWidth;
                    D.InstanceData.Size = D.InstanceData.File.Length;
                    D.NotifyPropertyChanged("InstanceData");
                }
                catch (Exception ex)
                {
                    D.InstanceData.File = null;
                    D.InstanceData.Height = 0;
                    D.InstanceData.Width = 0;
                    D.InstanceData.Size = 0;
                    D.NotifyPropertyChanged("InstanceData");
                    SQL.Error("Błąd podczas pobierania obrazu", ex, Config.GetModule(nameof(Modules.Icons)), D.InstanceData.ID, true, false);
                }
            });
        }

        /// <summary>
        /// Drop
        /// </summary>
        private void dpMain_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                D.InstanceData.Path = files[0];
                D.NotifyPropertyChanged("InstanceData");
            }
        }

        /// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
        {
            if (D.InstanceData.Width > Convert.ToInt32(Config.Icon_Dimensions_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalną szerokość: " + Config.Icon_Dimensions_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceData.Height > Convert.ToInt32(Config.Icon_Dimensions_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalną wysokość: " + Config.Icon_Dimensions_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceData.Size > Convert.ToInt32(Config.Icon_Size_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalny rozmiar: " + Config.Icon_Size_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceData.File == null)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano pliku z obrazem!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
