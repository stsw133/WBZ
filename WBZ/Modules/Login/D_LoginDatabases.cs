using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using WBZ.Models;

namespace WBZ.Modules.Login
{
    class D_LoginDatabases : INotifyPropertyChanged
    {
		/// Databases list
		private ObservableCollection<C_Database> databases = new ObservableCollection<C_Database>(C_Database.LoadAllDatabases());
		public ObservableCollection<C_Database> Databases
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

		/// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
