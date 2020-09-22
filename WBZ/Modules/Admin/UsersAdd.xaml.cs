using System;
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

		public UsersAdd(INSTANCE_CLASS instance, bool editMode)
		{
			InitializeComponent();
			DataContext = M;

			M.InstanceInfo = instance;
			M.EditMode = editMode;
		}
		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		#region buttons
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (SQL.SetUser(M.InstanceInfo))
				Close();
		}
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (M.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
		#endregion

		private void chckPerms_Checked(object sender, RoutedEventArgs e)
		{
			var perm = (sender as CheckBox).Tag.ToString();
			if (!M.InstanceInfo.Perms.Contains(perm))
				M.InstanceInfo.Perms.Add(perm);
		}

		private void chckPerms_Unchecked(object sender, RoutedEventArgs e)
		{
			var perm = (sender as CheckBox).Tag.ToString();
			if (M.InstanceInfo.Perms.Contains(perm))
				M.InstanceInfo.Perms.Remove(perm);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_UsersAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.ModuleTypes.USERS;

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
		/// Tryb edycji dla okna
		public bool EditMode { get; set; }
		/// Ikona okna
		public string EditIcon
		{
			get
			{
				if (InstanceInfo.ID == 0)
					return "pack://siteoforigin:,,,/Resources/icon32_add.ico";
				else if (EditMode)
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
				if (InstanceInfo.ID == 0)
					return "Nowy użytkownik";
				else if (EditMode)
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
