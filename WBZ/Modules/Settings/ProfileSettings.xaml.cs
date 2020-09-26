using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Modules.Settings
{
	/// <summary>
	/// Interaction logic for ProfileSettings.xaml
	/// </summary>
	public partial class ProfileSettings : Window
	{
        M_ProfileSettings M = new M_ProfileSettings();

        public ProfileSettings()
		{
			InitializeComponent();
            DataContext = M;
        }

		private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbOldPassword.Password) && !string.IsNullOrEmpty(tbNewPassword.Password) && !string.IsNullOrEmpty(tbRNewPassword.Password))
            {
                if (tbNewPassword.Password != tbRNewPassword.Password)
                {
                    MessageBox.Show("Nowe hasło nie zgadza się z powtórzeniem!");
                    return;
                }
                else
                {
                    if (SQL.Login(Global.User.Username, tbOldPassword.Password))
                        M.User.Newpass = tbNewPassword.Password;
                    else
                    {
                        MessageBox.Show("Podano złe dotychczasowe hasło!");
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
    internal class M_ProfileSettings : INotifyPropertyChanged
    {
        /// Dane o zalogowanym użytkowniku
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
