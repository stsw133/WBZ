using System.ComponentModel;

namespace WBZ.Modules.Login
{
    class D_LoginAppAbout
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// System version
		public string Version => StswExpress.Globals.Global.AppVersion();
		/// About app
		public string AboutApp => $@"To w pełni darmowy program biznesowy posiadający szeroki asortyment funkcjonalności. Oferuje modułowy sposób zarządzania zasobami i dystrybucją.";
		/// About creators
		public string AboutCreators => $@"Twórcami aplikacji jest dwójka studentów z Państwowej Wyższej Szkoły Zawodowej im. Hipolita Cegielskiego w Gnieźnie.";
	}
}
