using StswExpress;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WBZ.Login
{
    internal class D_Login : D
    {
        /// AppVersion
        public string AppVersion { get; } = Fn.AppVersion();
        
        /// Databases
        private ObservableCollection<DB> databases;
        public ObservableCollection<DB> Databases
        {
            get => databases;
            set => SetField(ref databases, value, () => Databases);
        }

        /// Status
        private ImageSource statusIcon = Fn.LoadImage(Properties.Resources.icon32_shield_white);
        public ImageSource StatusIcon
        {
            get => statusIcon;
            set => SetField(ref statusIcon, value, () => StatusIcon);
        }
        private string statusName;
        public string StatusName
        {
            get => statusName;
            set => SetField(ref statusName, value, () => StatusName);
        }
    }
}
