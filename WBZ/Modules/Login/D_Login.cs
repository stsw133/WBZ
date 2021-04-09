using StswExpress.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WBZ.Modules._base;

namespace WBZ.Modules.Login
{
    class D_Login : D
    {
		/// Databases list
		private ObservableCollection<M_Database> databases;
		public ObservableCollection<M_Database> Databases
		{
			get => databases;
			set => SetField(ref databases, value, () => Databases);
		}
	}
}
