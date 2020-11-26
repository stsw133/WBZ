using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using WBZ.Models;

namespace WBZ.Modules.Login
{
    class D_Login : INotifyPropertyChanged
    {
		/// Databases list
		private ObservableCollection<M_Database> databases;
		public ObservableCollection<M_Database> Databases
		{
			get
			{
				return databases;
			}
			set
			{
				databases = value;
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
