using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for MenuPanelColor.xaml
	/// </summary>
	public partial class MenuPanelColor : MenuItem
	{
		D_MenuPanelColor D = new D_MenuPanelColor();

		public MenuPanelColor()
		{
			InitializeComponent();
            DataContext = D;
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tag != null)
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(Tag.ToString());
                    D.R = color.R;
                    D.G = color.G;
                    D.B = color.B;
                    D.RGB = Tag.ToString();
                }
                catch { }
            }
        }

        private void customColor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                D.R = Convert.ToByte(R.Value);
                D.G = Convert.ToByte(G.Value);
                D.B = Convert.ToByte(B.Value);
                (sender as ComboBoxItem).Tag = $"#FF{D.R:X2}{D.G:X2}{D.B:X2}";
            }
            catch { }
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    class D_MenuPanelColor : INotifyPropertyChanged
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
