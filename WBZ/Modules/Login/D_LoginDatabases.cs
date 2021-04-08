using StswExpress.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WBZ.Modules.Login
{
    class D_LoginDatabases : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Databases list
		private ObservableCollection<M_Database> databases = new ObservableCollection<M_Database>(M_Database.LoadAllDatabases());
		public ObservableCollection<M_Database> Databases
		{
			get => databases;
			set
			{
				databases = value;
				NotifyPropertyChanged();
			}
		}
	}
}
