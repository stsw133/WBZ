using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for Versions.xaml
	/// </summary>
	public partial class Versions : Window
	{
		M_Versions M = new M_Versions();

		public Versions()
		{
			InitializeComponent();
			DataContext = M;
			btnRefresh_Click(null, null);
		}

		#region buttons
		private async void btnDownload_Click(object sender, MouseButtonEventArgs e)
		{
			string version = ((dynamic)lbList.SelectedValue).ToString();

			try
			{
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
							if (MessageBox.Show("Czy uruchomić plik instalacyjny? W przypadku akceptacji program zostanie wyłączony.", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
				MessageBox.Show(ex.Message);
			}
		}
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(Properties.Settings.Default.apiUrl))
				using (HttpContent content = response.Content)
				{
					string result = await content.ReadAsStringAsync();
					dynamic data = JObject.Parse(result);
					M.InstancesList = data;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		private void lbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				btnDownload_Click(null, null);
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_Versions : INotifyPropertyChanged
	{
		/// Lista instancji
		private dynamic instancesList;
		public dynamic InstancesList
		{
			get
			{
				return instancesList;
			}
			set
			{
				instancesList = value;
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
