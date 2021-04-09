using StswExpress.Globals;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
	/// <summary>
	/// Interaction logic for UsersNew.xaml
	/// </summary>
	public partial class UsersNew : New
	{
		D_UsersNew D = new D_UsersNew();

		public UsersNew(MODULE_MODEL instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			if (instance != null)
				D.InstanceInfo = instance;
			D.Mode = mode;

			D.InstanceInfo.Perms = SQL.GetUserPerms(D.InstanceInfo.ID);
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_TYPE);
		}

		/// <summary>
		/// Loaded
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			/*
			var coll = dpPerms.Children.Cast<UniformGrid>().OrderBy(x => (x.Children[0] as StswExpress.Controls.Header)?.Text).ToList();
			dpPerms.Children.RemoveRange(0, dpPerms.Children.Count);
			foreach (var item in coll)
				dpPerms.Children.Add(item);
			*/
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

		/// <summary>
		/// PasswordChanged
		/// </summary>
		private void tbNewpass_PasswordChanged(object sender, RoutedEventArgs e)
        {
			D.InstanceInfo.Newpass = (sender as PasswordBox).Password;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
