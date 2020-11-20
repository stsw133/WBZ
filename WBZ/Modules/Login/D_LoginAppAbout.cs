using System.ComponentModel;
using WBZ.Globals;

namespace WBZ.Modules.Login
{
    class D_LoginAppAbout
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
