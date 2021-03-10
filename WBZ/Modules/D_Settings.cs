using System.ComponentModel;
using System.Reflection;

namespace WBZ.Modules
{
    class D_Settings : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

        /// R
        private byte r;
        public byte R
        {
            get => r;
            set
            {
                r = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
                RGB = rgb;
            }
        }
        /// G
        private byte g;
        public byte G
        {
            get => g;
            set
            {
                g = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
                RGB = rgb;
            }
        }
        /// B
        private byte b;
        public byte B
        {
            get => b;
            set
            {
                b = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
                RGB = rgb;
            }
        }
        /// RGB
        private string rgb;
        public string RGB
        {
            get => rgb;
            set
            {
                rgb = $"#FF{r:X2}{g:X2}{b:X2}";
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
            }
        }
    }
}
