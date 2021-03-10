using System.ComponentModel;
using System.Reflection;

namespace WBZ.Modules.Login
{
    class D_Versions : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
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
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
	}
}
