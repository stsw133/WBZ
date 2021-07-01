using StswExpress;
using System.Windows;

namespace WBZ.Modules._base
{
    /// <summary>
    /// Interaction logic for MsgWin.xaml
    /// </summary>
    public partial class MsgWin : Window
    {
        public enum Types
        {
            MsgOnly = 0,
            InputBox = 1
        }
        public static class Titles
        {
            public const string BLOCKADE = "Blokada";
            public const string ERROR = "Błąd";
            public const string INFO = "Informacja";
            public const string QUESTION = "Pytanie";
            public const string WARNING = "Ostrzeżenie";
        }

        readonly D_MsgWin D = new D_MsgWin();

        public MsgWin(Types type, string title, string message = null, string value = null)
        {
            InitializeComponent();
            DataContext = D;

            D.Type = type;
            D.Title = title;
            D.Message = message;
            D.InputValue = value;
        }

        public string Value => D.InputValue;

        /// <summary>
        /// OK
        /// </summary>
        private void btnOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        /// <summary>
        /// Cancel
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }

    /// <summary>
    /// DataContext
    /// </summary>
    public class D_MsgWin : D
    {
        /// Type
        private MsgWin.Types type;
        public MsgWin.Types Type
        {
            get => type;
            set => SetField(ref type, value, () => Type);
        }

        /// Title
        private string title;
        public string Title
        {
            get => title;
            set => SetField(ref title, value, () => Title);
        }

        /// Message
        private string message;
        public string Message
        {
            get => message;
            set => SetField(ref message, value, () => Message);
        }

        /// InputValue
        private string inputValue;
        public string InputValue
        {
            get => inputValue ?? string.Empty;
            set => SetField(ref inputValue, value, () => InputValue);
        }

        /// Icon
        public string Icon => Title switch
        {
            MsgWin.Titles.BLOCKADE => "/Resources/32/icon32_shield_orange.ico",
            MsgWin.Titles.ERROR => "/Resources/32/icon32_shield_red.ico",
            MsgWin.Titles.INFO => "/Resources/32/icon32_shield_green.ico",
            MsgWin.Titles.QUESTION => "/Resources/32/icon32_shield_blue.ico",
            MsgWin.Titles.WARNING => "/Resources/32/icon32_shield_yellow.ico",
            _ => null,
        };
    }
}
