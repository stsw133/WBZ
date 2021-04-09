using System.ComponentModel;
using System.Runtime.CompilerServices;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;

namespace WBZ.Modules.Personal
{
    class D_Profile : D
    {
        /// Logged user
        private M_User user = Global.User;
        public M_User User
        {
            get => user;
            set => SetField(ref user, value, () => User);
        }
    }
}
