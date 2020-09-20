using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WBZ.Classes;
using WBZ.Helpers;

namespace WBZ.Controls
{
    /// <summary>
    /// Interaction logic for AttributesTab.xaml
    /// </summary>
    public partial class AttributesTab : UserControl
    {
        M_AttributesTab M = new M_AttributesTab();
        private string InstanceType;
        private int ID;
        private bool EditMode;

        public AttributesTab()
        {
            InitializeComponent();
            DataContext = M;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = Window.GetWindow(this);

                if (ID != 0 && M.InstanceAttributes == null)
                    M.InstanceAttributes = SQL.ListAttributes(InstanceType, ID);

                dynamic d = win?.DataContext;
                if (d != null)
                {
                    InstanceType = (string)d.INSTANCE_TYPE;
                    ID = (int)d.InstanceInfo.ID;
                    EditMode = (bool)d.EditMode;
                }
            }
            catch { }
        }

        private void btnAttributeChange_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = Content as DataGrid;

            var indexes = dataGrid.SelectedItems.Cast<C_Attribute>().Select(x => M.InstanceAttributes.IndexOf(x));
            foreach (int index in indexes)
            {
                var window = new AttributeValueChange(M.InstanceAttributes[index], EditMode);
                window.Owner = (((Parent as TabItem)?.Parent as TabControl)?.Parent as DockPanel)?.Parent as Window;
                if (window.ShowDialog() == true)
                    SQL.UpdateAttribute(M.InstanceAttributes[index]);
            }

            M.InstanceAttributes = SQL.ListAttributes(InstanceType, ID);
        }
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_AttributesTab : INotifyPropertyChanged
    {
        /// Atrybuty
        private List<C_Attribute> instanceAttributes;
        public List<C_Attribute> InstanceAttributes
        {
            get
            {
                return instanceAttributes;
            }
            set
            {
                instanceAttributes = value;
                NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
