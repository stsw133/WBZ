using StswExpress.Globals;
using WBZ.Modules._base;

namespace WBZ.Modules
{
    class D_Main : D
    {
		/// Title
		public string Title { get; } = Global.AppDatabase.Name + TranslateMe.TM.Tr("Main", " - okno główne", StswExpress.Globals.Properties.Language);

		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
