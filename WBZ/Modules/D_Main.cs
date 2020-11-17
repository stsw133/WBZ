using System.ComponentModel;
using WBZ.Helpers;

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
		public string Title { get; } = Global.Database.Name + " - okno główne";
		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
