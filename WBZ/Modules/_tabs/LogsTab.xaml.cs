using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Globals;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using WBZ.Modules._base;

namespace WBZ.Modules._tabs
{
    /// <summary>
    /// Interaction logic for LogsTab.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        D_LogsTab D = new D_LogsTab();
        private string Module;
        private int ID;

        public LogsTab()
        {
            InitializeComponent();
            DataContext = D;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && D.InstanceLogs == null)
                    D.InstanceLogs = SQL.ListInstances<M_Log>(M_Module.Module.LOGS, $"l.module='{Module}' and l.instance={ID}");

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    Module = (string)d.MODULE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                }
            }
            catch { }
        }
	}

	/// <summary>
	/// DataContext
	/// </summary>
	class D_LogsTab : D
    {
        /// Logs
        private ObservableCollection<M_Log> instanceLogs;
        public ObservableCollection<M_Log> InstanceLogs
        {
            get => instanceLogs;
            set => SetField(ref instanceLogs, value, () => InstanceLogs);
        }
    }
}
