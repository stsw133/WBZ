using StswExpress;
using System.Windows;
using System.Windows.Media;

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
        private void BtnOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        /// <summary>
        /// Cancel
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
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
        public ImageSource Icon => Title switch
        {
            MsgWin.Titles.BLOCKADE => Fn.LoadImage(Properties.Resources.icon32_shield_orange),
            MsgWin.Titles.ERROR => Fn.LoadImage(Properties.Resources.icon32_shield_red),
            MsgWin.Titles.INFO => Fn.LoadImage(Properties.Resources.icon32_shield_green),
            MsgWin.Titles.QUESTION => Fn.LoadImage(Properties.Resources.icon32_shield_blue),
            MsgWin.Titles.WARNING => Fn.LoadImage(Properties.Resources.icon32_shield_yellow),
            _ => null
        };
    }
}
