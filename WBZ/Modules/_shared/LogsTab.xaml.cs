using StswExpress;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WBZ.Models;
using WBZ.Globals;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for LogsTab.xaml
    /// </summary>
    public partial class LogsTab : UserControl
    {
        readonly D_LogsTab D = new D_LogsTab();

        public LogsTab()
        {
            InitializeComponent();
            //DataContext = D;
        }

        /// <summary>
        /// Module
        /// </summary>
        public static readonly DependencyProperty ModuleProperty
            = DependencyProperty.Register(
                  nameof(Module),
                  typeof(MV),
                  typeof(LogsTab),
                  new PropertyMetadata(default(MV))
              );
        public MV Module
        {
            get => (MV)GetValue(ModuleProperty);
            set => SetValue(ModuleProperty, value);
        }

        /// <summary>
        /// InstanceID
        /// </summary>
        public static readonly DependencyProperty InstanceIDProperty
            = DependencyProperty.Register(
                  nameof(InstanceID),
                  typeof(int),
                  typeof(LogsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InstanceID > 0 && D.InstanceLogs == null)
                    D.InstanceLogs = SQL.ListInstances<M_Log>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
                DataContext = D;
                Loaded -= UserControl_Loaded;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd inicjalizacji zakładki logów", ex, Module, InstanceID);
            }
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    internal class D_LogsTab : D
    {
        /// Module
        public MV Module = Config.GetModule(nameof(Logs));

        /// Logs
        private List<M_Log> instanceLogs;
        public List<M_Log> InstanceLogs
        {
            get => instanceLogs;
            set => SetField(ref instanceLogs, value, () => InstanceLogs);
        }
    }
}
