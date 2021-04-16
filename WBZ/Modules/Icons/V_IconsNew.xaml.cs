using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WBZ.Controls;
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
        D_IconsNew D = new D_IconsNew();

        public IconsNew(MODULE_MODEL instance, StswExpress.Globals.Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;
        }

        /// <summary>
        /// GetHyperLink
        /// </summary>
        private void btnGetHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StswExpress.Globals.Functions.OpenFile("https://www.google.com/");
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
                D.InstanceInfo.Path = dialog.FileName;
                D.NotifyPropertyChanged("InstanceInfo");
            }
        }

        /// <summary>
        /// TextChanged
        /// </summary>
        private async void tbPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            D.InstanceInfo.Path = (sender as TextBox).Text;
            await Task.Run(() =>
            {
                try
                {
                    if (D.InstanceInfo.Path.StartsWith("http"))
                    {
                        using (var client = new WebClient())
                        {
                            D.InstanceInfo.File = client.DownloadData(D.InstanceInfo.Path);
                        }
                    }
                    else
                        D.InstanceInfo.File = File.ReadAllBytes(D.InstanceInfo.Path);

                    D.InstanceInfo.Format = D.InstanceInfo.Path.Split('.').Last();
                    if (string.IsNullOrEmpty(D.InstanceInfo.Name))
                        D.InstanceInfo.Name = Path.GetFileName(D.InstanceInfo.Path).TrimEnd(D.InstanceInfo.Format.ToCharArray());

                    var image = StswExpress.Globals.Functions.LoadImage(D.InstanceInfo.File);
                    
                    D.InstanceInfo.Height = image.PixelHeight;
                    D.InstanceInfo.Width = image.PixelWidth;
                    D.InstanceInfo.Size = D.InstanceInfo.File.Length;
                    D.NotifyPropertyChanged("InstanceInfo");
                }
                catch (Exception ex)
                {
                    D.InstanceInfo.File = null;
                    D.InstanceInfo.Height = 0;
                    D.InstanceInfo.Width = 0;
                    D.InstanceInfo.Size = 0;
                    D.NotifyPropertyChanged("InstanceInfo");
                    SQL.Error("Błąd podczas pobierania obrazu", ex, Config.Modules.ICONS, D.InstanceInfo.ID, true, false);
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
                D.InstanceInfo.Path = files[0];
                D.NotifyPropertyChanged("InstanceInfo");
            }
        }

        /// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
        {
            if (D.InstanceInfo.Width > Convert.ToInt32(Config.Icon_Dimensions_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalną szerokość: " + Config.Icon_Dimensions_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceInfo.Height > Convert.ToInt32(Config.Icon_Dimensions_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalną wysokość: " + Config.Icon_Dimensions_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceInfo.Size > Convert.ToInt32(Config.Icon_Size_Max))
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Obraz przekracza dopuszczalny rozmiar: " + Config.Icon_Size_Max) { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceInfo.File == null)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.BLOCKADE, "Nie wybrano pliku z obrazem!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
