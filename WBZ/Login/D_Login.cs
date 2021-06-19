using StswExpress;
using System.Collections.ObjectModel;

namespace WBZ.Login
{
    class D_Login : D
	{
		/// <summary>
		/// Databases
		/// </summary>
		private ObservableCollection<DB> databases;
		public ObservableCollection<DB> Databases
		{
			get => databases;
			set => SetField(ref databases, value, () => Databases);
		}
	}
}
