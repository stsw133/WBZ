using StswExpress;
using System.Collections.ObjectModel;

namespace WBZ.Modules.Login
{
    class D_LoginDatabases : D
    {
		/// Databases list
		private ObservableCollection<DB> databases = new ObservableCollection<DB>(DB.LoadAllDatabases());
		public ObservableCollection<DB> Databases
		{
			get => databases;
			set => SetField(ref databases, value, () => Databases);
		}
	}
}
