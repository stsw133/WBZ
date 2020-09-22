using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Modules.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesAdd.xaml
    /// </summary>
    public partial class EmployeesAdd : Window
    {
        M_EmployeesAdd M = new M_EmployeesAdd();

        public EmployeesAdd(C_Employee instance, bool editMode)
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

			if (SQL.SetEmployee(M.InstanceInfo))
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

		private void btnUserChoose_Click(object sender, RoutedEventArgs e)
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
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_EmployeesAdd : INotifyPropertyChanged
	{
		public readonly string INSTANCE_TYPE = Global.ModuleTypes.EMPLOYEES;

		/// Dane o zalogowanym użytkowniku
		public C_User User { get; } = Global.User;
		/// Instancja
		private C_Employee instanceInfo;
		public C_Employee InstanceInfo
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
		public bool IsEditing { get { return InstanceInfo.ID > 0; } }
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
					return "Nowy pracownik";
				else if (EditMode)
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
