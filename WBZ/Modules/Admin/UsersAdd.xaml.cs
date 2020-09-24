using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using INSTANCE_CLASS = WBZ.Classes.C_User;

namespace WBZ.Modules.Admin
{
	/// <summary>
	/// Interaction logic for AdminUsersAdd.xaml
	/// </summary>
	public partial class UsersAdd : Window
	{
		M_UsersAdd M = new M_UsersAdd();

		public UsersAdd(INSTANCE_CLASS instance, Global.ActionType mode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.Mode = mode;

			if (M.Mode == Global.ActionType.NEW)
				M.InstanceInfo.ID = SQL.NewInstance(M.INSTANCE_TYPE);
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
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (SQL.SetInstance(M.INSTANCE_TYPE, M.InstanceInfo))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (M.InstanceInfo.ID == 0)
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
			if (!M.InstanceInfo.Perms.Contains(perm))
				M.InstanceInfo.Perms.Add(perm);
		}

		/// <summary>
		/// Remove perm
		/// </summary>
		private void chckPerms_Unchecked(object sender, RoutedEventArgs e)
		{
			var perm = (sender as CheckBox).Tag.ToString();
			if (M.InstanceInfo.Perms.Contains(perm))
				M.InstanceInfo.Perms.Remove(perm);
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_UsersAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.Module.USERS;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private INSTANCE_CLASS instanceInfo;
		public INSTANCE_CLASS InstanceInfo
		{
			get
			{
				return instanceInfo;
			}
			set
			{
				instanceInfo = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Czy okno jest w trybie edycji (zamiast w trybie dodawania)
		public bool IsEditing { get { return Mode != Global.ActionType.NEW; } }
		/// Tryb okna
		public Global.ActionType Mode { get; set; }
		/// Dodatkowa ikona okna
		public string ModeIcon
		{
			get
			{
				if (Mode == Global.ActionType.ADD)
					return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
				else if (Mode == Global.ActionType.DUPLICATE)
					return "pack://siteoforigin:,,,/Resources/icon32_duplicate.ico";
				else if (Mode == Global.ActionType.EDIT)
					return "pack://siteoforigin:,,,/Resources/icon32_edit.ico";
				else
					return "pack://siteoforigin:,,,/Resources/icon32_search.ico";
			}
		}
		/// Tytuł okna
		public string Title
		{
			get
			{
				if (Mode == Global.ActionType.ADD)
					return "Nowy użytkownik";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie użytkownika: {InstanceInfo.Fullname}";
				else if (Mode == Global.ActionType.EDIT)
					return $"Edycja użytkownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd użytkownika: {InstanceInfo.Fullname}";
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
