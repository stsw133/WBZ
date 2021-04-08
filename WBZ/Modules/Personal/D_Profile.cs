using System.ComponentModel;
using System.Runtime.CompilerServices;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules.Personal
{
    class D_Profile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// Logged user
        private M_User user = Global.User;
        public M_User User
        {
            get => user;
            set
            {
                user = value;
                NotifyPropertyChanged();
            }
        }
    }
}
