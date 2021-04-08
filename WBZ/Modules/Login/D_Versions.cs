using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WBZ.Modules.Login
{
    class D_Versions : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Instances list
		private dynamic instancesList;
		public dynamic InstancesList
		{
			get => instancesList;
			set
			{
				instancesList = value;
				NotifyPropertyChanged();
			}
		}
	}
}
