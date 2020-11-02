using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_User;

namespace WBZ.Modules.Users
{
	/// <summary>
	/// Interaction logic for UsersNew.xaml
	/// </summary>
	public partial class UsersNew : Window
	{
		D_UsersNew D = new D_UsersNew();

		public UsersNew(MODULE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = D;

			D.InstanceInfo = instance;
			D.Mode = mode;

			D.InstanceInfo.Perms = SQL.GetUserPerms(D.InstanceInfo.ID);
			if (D.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
		}

		/// <summary>
		/// Validation
		/// </summary>
		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (D.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Add perm
		/// </summary>
		private void chckPerms_Checked(object sender, RoutedEventArgs e)
		{
			var perm = (sender as CheckBox).Tag.ToString();
			if (!D.InstanceInfo.Perms.Contains(perm))
				D.InstanceInfo.Perms.Add(perm);
		}

		/// <summary>
		/// Remove perm
		/// </summary>
		private void chckPerms_Unchecked(object sender, RoutedEventArgs e)
		{
			var perm = (sender as CheckBox).Tag.ToString();
			if (D.InstanceInfo.Perms.Contains(perm))
				D.InstanceInfo.Perms.Remove(perm);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (D.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
