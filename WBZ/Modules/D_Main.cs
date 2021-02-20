using System.ComponentModel;
using WBZ.Globals;

namespace WBZ.Modules
{
    class D_Main : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Title
		public string Title { get; } = Global.Database.Name + TranslateMe.TM.Tr("Main", " - okno główne", Global.Language);
		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
