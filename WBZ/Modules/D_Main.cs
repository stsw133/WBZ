using StswExpress.Base;
using StswExpress.Globals;
using StswExpress.Translate;

namespace WBZ.Modules
{
    class D_Main : D
    {
		/// Title
		public string Title => Global.AppDatabase.Name + TM.Tr("MainWindow");

		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
