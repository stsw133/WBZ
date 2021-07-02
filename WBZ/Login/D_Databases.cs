using StswExpress;
using System.Collections.ObjectModel;

namespace WBZ.Login
{
    internal class D_Databases : D
    {
        /// Databases
        private ObservableCollection<DB> databases = new ObservableCollection<DB>(DB.LoadAllDatabases());
        public ObservableCollection<DB> Databases
        {
            get => databases;
            set => SetField(ref databases, value, () => Databases);
        }

        /// Can...
        private bool canUpdateDatabase = false;
        public bool CanUpdateDatabase
        {
            get => canUpdateDatabase;
            set => SetField(ref canUpdateDatabase, value, () => CanUpdateDatabase);
        }
        private bool canCreateAdmin = false;
        public bool CanCreateAdmin
        {
            get => canCreateAdmin;
            set => SetField(ref canCreateAdmin, value, () => CanCreateAdmin);
        }
    }
}
