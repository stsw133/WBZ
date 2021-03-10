using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Globals;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for Versions.xaml
	/// </summary>
	public partial class Versions : Window
	{
		D_Versions D = new D_Versions();

		public Versions()
		{
			InitializeComponent();
			DataContext = D;
		}

		/// <summary>
		/// Loaded
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cmdRefresh_Executed(null, null);
		}

		/// <summary>
		/// Download
		/// </summary>
		private async void btnDownload_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string version = ((dynamic)lbList.SelectedValue).ToString();

				var dialog = new SaveFileDialog()
				{
					Filter = "Plik instalacyjny|*.exe"
				};
				dialog.FileName = Path.Combine(dialog.InitialDirectory, $"WBZ {version} install.exe");

				if (dialog.ShowDialog() == true)
				{
					using (HttpClient client = new HttpClient())
					using (HttpResponseMessage response = await client.GetAsync($"{Properties.Settings.Default.apiUrl}/{version}"))
					using (HttpContent content = response.Content)
					{
						string result = await content.ReadAsStringAsync();
						dynamic data = JObject.Parse(result);
						var file = (byte[])data.software;

						if (file != null)
						{
							File.WriteAllBytes(dialog.FileName, file);
							if (new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.INFO, "Czy uruchomić plik instalacyjny? W przypadku akceptacji program zostanie wyłączony.") { Owner = this }.ShowDialog() == true)
							{
								Process.Start(dialog.FileName);
								App.Current.Shutdown();
							}
						}
						else
							throw new Exception("Nie powiodło się pobieranie pliku instalacyjnego!");
					}
				}
			}
			catch (Exception ex)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd pobierania wersji: " + ex.Message) { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(Properties.Settings.Default.apiUrl))
				using (HttpContent content = response.Content)
				{
					string result = await content.ReadAsStringAsync();
					dynamic data = JObject.Parse(result);
					D.InstancesList = data;
				}
			}
			catch (Exception ex)
			{
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd odświeżania listy: " + ex.Message) { Owner = this }.ShowDialog();
			}
		}

		/// <summary>
		/// Help
		/// </summary>
		private void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Functions.OpenHelp(this);
		}

		/// <summary>
		/// Close
		/// </summary>
		private void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// VersionList - MouseDoubleClick
		/// </summary>
		private void lbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				btnDownload_Click(null, null);
		}
    }
}
