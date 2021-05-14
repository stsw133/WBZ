using StswExpress.Translate;
using StswExpress;

namespace WBZ.Modules
{
    class D_Main : D
    {
		/// Title
		public string Title => Fn.AppDatabase.Name + TM.Tr("MainWindow");

		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
