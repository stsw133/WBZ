using StswExpress.Models;
using System.Collections.ObjectModel;
using WBZ.Models;

namespace WBZ.Modules.Login
{
    class D_LoginDatabases : D
    {
		/// Databases list
		private ObservableCollection<M_Database> databases = new ObservableCollection<M_Database>(M_Database.LoadAllDatabases());
		public ObservableCollection<M_Database> Databases
		{
			get => databases;
			set => SetField(ref databases, value, () => Databases);
		}
	}
}
