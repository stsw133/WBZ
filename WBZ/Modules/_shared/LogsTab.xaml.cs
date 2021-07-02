using StswExpress;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Globals;
using WBZ.Modules._base;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for LogsTab.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        readonly D_LogsTab D = new D_LogsTab();

        private MV Module;
        private int InstanceID;

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
                var win = Window.GetWindow(this);
                var d = win?.DataContext as D_ModuleNew<dynamic>;

                if (d != null)
                {
                    Module = d.Module;
                    InstanceID = (d.InstanceData as M).ID;
                }
                if (InstanceID != 0 && D.InstanceLogs == null)
                    D.InstanceLogs = SQL.ListInstances<M_Log>(D.ModuleLogs, $"{D.ModuleLogs.Alias}.module_alias='{Module.Alias}' and {D.ModuleLogs.Alias}.instance_id={InstanceID}");
            }
            catch { }
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    internal class D_LogsTab : D
    {
        /// Module
        public MV ModuleLogs = Config.GetModule(nameof(Logs));

        /// Logs
        private List<M_Log> instanceLogs;
        public List<M_Log> InstanceLogs
        {
            get => instanceLogs;
            set => SetField(ref instanceLogs, value, () => InstanceLogs);
        }
    }
}
