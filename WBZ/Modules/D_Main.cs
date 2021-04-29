using StswExpress.Base;
using StswExpress.Globals;

namespace WBZ.Modules
{
    class D_Main : D
    {
		/// Title
		public string Title => Global.AppDatabase.Name + TranslateMe.TM.Tr("Main", " - okno główne", StswExpress.Globals.Properties.Language);

		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
