using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using WBZ.Modules._base;

namespace WBZ.Login
{
    /// <summary>
    /// Interaction logic for Versions.xaml
    /// </summary>
    public partial class Versions : Window
    {
        readonly D_Versions D = new D_Versions();

        public Versions()
        {
            InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e) => cmdRefresh_Executed(null, null);

        /// <summary>
        /// Download
        /// </summary>
        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string version = LstBoxList.SelectedValue.ToString();

                var dialog = new SaveFileDialog()
                {
                    Filter = "Plik instalacyjny|*.exe"
                };
                dialog.FileName = Path.Combine(dialog.InitialDirectory, $"WBZ {version} install.exe");

                if (dialog.ShowDialog() == true)
                {
                    using (var client = new HttpClient())
                    using (var response = await client.GetAsync($"{Properties.Settings.Default.apiUrl}/{version}"))
                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        dynamic data = JObject.Parse(result);
                        var file = (byte[])data.software;

                        if (file != null)
                        {
                            File.WriteAllBytes(dialog.FileName, file);
                            if (new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.INFO, "Czy uruchomić plik instalacyjny? W przypadku akceptacji program zostanie wyłączony.") { Owner = this }.ShowDialog() == true)
                            {
                                Process.Start(dialog.FileName);
                                App.Current.Shutdown();
                            }
                        }
                        else throw new Exception("Nie powiodło się pobieranie pliku instalacyjnego!");
                    }
                }
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd pobierania wersji:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        private async void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(Properties.Settings.Default.apiUrl))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    dynamic data = JObject.Parse(result);
                    D.InstancesList = data;
                }
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.ERROR, $"Błąd odświeżania listy:{Environment.NewLine}{ex.Message}") { Owner = this }.ShowDialog();
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        private void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        /// <summary>
        /// VersionList - MouseDoubleClick
        /// </summary>
        private void LstBoxList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                btnDownload_Click(null, null);
        }
    }
}
