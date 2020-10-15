using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Controls;
using WBZ.Helpers;

namespace WBZ.Modules.Personal
{
	/// <summary>
	/// Interaction logic for Profile.xaml
	/// </summary>
	public partial class Profile : Window
	{
        M_Profile M = new M_Profile();

        public Profile()
		{
			InitializeComponent();
            DataContext = M;
        }

        /// <summary>
        /// Save
        /// </summary>
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbOldPassword.Password) && !string.IsNullOrEmpty(tbNewPassword.Password) && !string.IsNullOrEmpty(tbRNewPassword.Password))
            {
                if (tbNewPassword.Password != tbRNewPassword.Password)
                {
                    new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Nowe hasło nie zgadza się z powtórzeniem!") { Owner = this }.ShowDialog();
                    return;
                }
                else
                {
                    if (SQL.Login(Global.User.Username, tbOldPassword.Password))
                        M.User.Newpass = tbNewPassword.Password;
                    else
                    {
                        new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Podano złe dotychczasowe hasło!") { Owner = this }.ShowDialog();
                        return;
                    }
                }
            }

            if (SQL.SetInstance(Global.Module.USERS, M.User, Global.ActionType.EDIT))
			    Close();
		}
	}

    /// <summary>
    /// Model
    /// </summary>
    internal class M_Profile : INotifyPropertyChanged
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
