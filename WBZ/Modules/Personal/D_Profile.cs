using System.ComponentModel;
using System.Reflection;
using WBZ.Helpers;
using WBZ.Models;

namespace WBZ.Modules.Personal
{
    class D_Profile : INotifyPropertyChanged
    {
        /// Logged user
        private C_User user = Global.User;
        public C_User User
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
