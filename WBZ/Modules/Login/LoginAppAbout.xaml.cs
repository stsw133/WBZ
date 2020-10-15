using System.ComponentModel;
using System.Windows;
using WBZ.Helpers;

namespace WBZ.Modules.Login
{
	/// <summary>
	/// Interaction logic for LoginAppAbout.xaml
	/// </summary>
	public partial class LoginAppAbout : Window
	{
		M_LoginAppAbout M = new M_LoginAppAbout();

		public LoginAppAbout()
		{
			InitializeComponent();
			DataContext = M;
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_LoginAppAbout : INotifyPropertyChanged
	{
		/// System version
		public string Version => Global.Version;
		/// About app
		public string AboutApp => $@"To w pełni darmowy program wspomagający pracę banków żywności. Oferuje modułowy sposób zarządzania zasobami i dystrybucją.";
		/// About creators
		public string AboutCreators => $@"Twórcami aplikacji jest dwójka studentów z Państwowej Wyższej Szkoły Zawodowej im. Hipolita Cegielskiego w Gnieźnie.";

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
