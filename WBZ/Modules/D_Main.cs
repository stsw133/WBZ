using StswExpress.Translate;
using StswExpress;

namespace WBZ.Modules
{
    class D_Main : D
    {
		/// <summary>
		/// Title
		/// </summary>
		public string Title => Fn.AppDatabase.Name + TM.Tr("MainWindow");

		/// <summary>
		/// WantToLogout
		/// </summary>
		public bool WantToLogout { get; set; }
	}
}
