using StswExpress;
using WBZ.Globals;
using WBZ.Models;

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
