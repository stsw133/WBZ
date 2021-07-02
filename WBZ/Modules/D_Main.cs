using StswExpress.Translate;
using StswExpress;

namespace WBZ.Modules
{
    internal class D_Main : D
    {
        /// Title
        public string Title => Fn.AppDatabase.Name + TM.Tr("MainWindow");

        /// WantToLogout
        public bool WantToLogout { get; set; }
    }
}
