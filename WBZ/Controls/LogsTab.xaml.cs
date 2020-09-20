using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WBZ.Classes;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for LogsTab.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        M_LogsTab M = new M_LogsTab();
        private string InstanceType;
        private int ID;
        private bool EditMode;

        public LogsTab()
        {
            InitializeComponent();
            DataContext = M;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && M.InstanceLogs == null)
                    M.InstanceLogs = SQL.ListLogs(InstanceType, ID, null);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    InstanceType = (string)d.INSTANCE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                    EditMode = (bool)d.EditMode;
                }
            }
            catch { }
        }
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_LogsTab : INotifyPropertyChanged
    {
        /// Logi
        private List<C_Log> instanceLogs;
        public List<C_Log> InstanceLogs
        {
            get
            {
                return instanceLogs;
            }
            set
            {
                instanceLogs = value;
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
