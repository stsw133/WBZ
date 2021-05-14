using StswExpress;
using System.Collections.ObjectModel;

namespace WBZ.Modules.Login
{
    class D_Login : D
	{
		/// Databases list
		private ObservableCollection<DB> databases;
		public ObservableCollection<DB> Databases
		{
			get => databases;
			set => SetField(ref databases, value, () => Databases);
		}
	}
}
