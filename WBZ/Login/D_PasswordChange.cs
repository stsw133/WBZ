using StswExpress;

namespace WBZ.Login
{
    internal class D_PasswordChange : D
    {
        /// E-mail
        private string email;
        public string Email
        {
            get => email;
            set => SetField(ref email, value, () => Email);
        }
    }
}
