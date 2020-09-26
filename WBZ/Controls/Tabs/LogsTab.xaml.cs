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
        private string Module;
        private int ID;

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
                    M.InstanceLogs = SQL.ListLogs(Module, ID, null);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_NAME;
                    ID = (int)d.InstanceInfo.ID;
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
        /// Logs
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
