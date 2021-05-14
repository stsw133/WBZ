using StswExpress;

namespace WBZ.Modules.Personal
{
    class D_Profile : D
    {
        /// Logged user
        private Models.M_User user = Globals.Global.User;
        public Models.M_User User
        {
            get => user;
            set => SetField(ref user, value, () => User);
        }
    }
}
