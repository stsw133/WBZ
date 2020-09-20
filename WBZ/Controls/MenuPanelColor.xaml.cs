using System.Windows;
using System.Windows.Controls;
using WBZ.Helpers;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for MenuPanelColor.xaml
	/// </summary>
	public partial class MenuPanelColor : UserControl
	{
		public MenuPanelColor()
		{
			InitializeComponent();
		}

		private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
		{
			var window = new MsgWin(MsgWin.Type.InputBox, "Własny kolor", "Podaj kod szesnastkowy swojego koloru:", Tag.ToString());
			if (window.ShowDialog() == true)
				Tag = window.values[0];
		}
	}
}
