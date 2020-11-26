using System.ComponentModel;
using System.Reflection;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules.Personal
{
    class D_Profile : INotifyPropertyChanged
    {
        /// Logged user
        private M_User user = Global.User;
        public M_User User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// PropertyChangedEventHandler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
