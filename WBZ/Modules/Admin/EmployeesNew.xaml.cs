using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Classes.C_Employee;

namespace WBZ.Modules.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesAdd.xaml
    /// </summary>
    public partial class EmployeesNew : Window
    {
        M_EmployeesNew M = new M_EmployeesNew();

        public EmployeesNew(MODULE_CLASS instance, Global.ActionType mode)
        {
            InitializeComponent();
            DataContext = M;

            M.InstanceInfo = instance;
			M.Mode = mode;

			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE))
				M.InstanceInfo.ID = SQL.NewInstanceID(M.MODULE_NAME);
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

			if (saved = SQL.SetInstance(M.MODULE_NAME, M.InstanceInfo, M.Mode))
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
		/// Select: User
		/// </summary>
		private void btnSelectUser_Click(object sender, RoutedEventArgs e)
		{
			var window = new UsersList(true);
			if (window.ShowDialog() == true)
				if (window.Selected != null)
				{
					M.InstanceInfo.User = window.Selected.ID;
					M.InstanceInfo.UserName = window.Selected.Fullname;
					M.InstanceInfo = M.InstanceInfo;
				}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (M.Mode.In(Global.ActionType.NEW, Global.ActionType.DUPLICATE) && !saved)
				SQL.ClearObject(M.MODULE_NAME, M.InstanceInfo.ID);
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_EmployeesNew : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.EMPLOYEES;

		/// Logged user
		public C_User User { get; } = Global.User;
		/// Instance
		private MODULE_CLASS instanceInfo;
		public MODULE_CLASS InstanceInfo
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
		/// Editing mode
		public bool EditingMode { get { return Mode != Global.ActionType.PREVIEW; } }
		/// Tryb okna
		public Global.ActionType Mode { get; set; }
		/// Dodatkowa ikona okna
		public string ModeIcon
		{
			get
			{
				if (Mode == Global.ActionType.NEW)
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
				if (Mode == Global.ActionType.NEW)
					return "Nowy pracownik";
				else if (Mode == Global.ActionType.DUPLICATE)
					return $"Duplikowanie pracownika: {InstanceInfo.Fullname}";
				else if (Mode == Global.ActionType.EDIT)
					return $"Edycja pracownika: {InstanceInfo.Fullname}";
				else
					return $"Podgląd pracownika: {InstanceInfo.Fullname}";
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
