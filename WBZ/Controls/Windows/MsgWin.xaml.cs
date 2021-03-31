using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for MsgWin.xaml
	/// </summary>
	public partial class MsgWin : Window
	{
		public enum Type
		{
			MsgOnly = 0,
			InputBox = 1
		}
		public static class MsgTitle
		{
			public const string BLOCKADE = "Blokada";
			public const string ERROR = "Błąd";
			public const string INFO = "Informacja";
			public const string QUESTION = "Pytanie";
			public const string WARNING = "Ostrzeżenie";
		}

		D_MsgWin D = new D_MsgWin();

		public MsgWin(Type type, string title, string message = "", string value = "")
		{
			InitializeComponent();
			DataContext = D;

			D.Title = title;
			txtMessage.Text = message;
			tbInput.Text = value;

			///InputBox
			if (type == Type.InputBox)
				tbInput.Visibility = Visibility.Visible;
		}

		public string Value { get => tbInput.Text; }

		/// <summary>
		/// OK
		/// </summary>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		/// <summary>
		/// Cancel
		/// </summary>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}

	/// <summary>
	/// DataContext
	/// </summary>
	public class D_MsgWin : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Title
		private string title;
		public string Title
		{
			get => title;
			set
			{
				title = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
		/// Icon
		public string Icon
		{
			get
			{
				if (Title == MsgWin.MsgTitle.BLOCKADE)
					return "/Resources/icon32_shield_orange.ico";
				else if (Title == MsgWin.MsgTitle.ERROR)
					return "/Resources/icon32_shield_red.ico";
				else if (Title == MsgWin.MsgTitle.INFO)
					return "/Resources/icon32_shield_green.ico";
				else if (Title == MsgWin.MsgTitle.QUESTION)
					return "/Resources/icon32_shield_blue.ico";
				else if (Title == MsgWin.MsgTitle.WARNING)
					return "/Resources/icon32_shield_yellow.ico";
				else
					return null;
			}
		}
	}
}
