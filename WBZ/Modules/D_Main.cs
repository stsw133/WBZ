using System.ComponentModel;
using WBZ.Helpers;

namespace WBZ.Modules
{
    class D_Main : INotifyPropertyChanged
    {
		/// PropertyList
		public string Title { get; } = Global.Database.Name + " - okno główne";
		public bool WantToLogout { get; set; } = false;

		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
