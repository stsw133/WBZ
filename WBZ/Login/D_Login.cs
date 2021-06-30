using StswExpress;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WBZ.Login
{
    class D_Login : D
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
        private ImageSource statusIcon = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/32/icon32_shield_white.ico"));
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
