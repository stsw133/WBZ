using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WBZ.Modules
{
    class D_Settings : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
    }
}
