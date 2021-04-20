using StswExpress.Globals;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Modules.Icons;

namespace WBZ.Modules._tabs
{
    /// <summary>
    /// Interaction logic for AdditionalsTab.xaml
    /// </summary>
    public partial class AdditionalsTab : UserControl
    {
        public AdditionalsTab()
        {
            InitializeComponent();
        }

        public bool HasIcon
        {
            get => (bool)GetValue(pHasIcon);
            set { SetValue(pHasIcon, value); }
        }
        public static readonly DependencyProperty pHasIcon
            = DependencyProperty.Register(
                  nameof(HasIcon),
                  typeof(bool),
                  typeof(GroupsView),
                  new PropertyMetadata(true)
              );

        /// <summary>
        /// ManageIcon - drop image
        /// </summary>
        private void btnManageIcon_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                byte[] file = File.ReadAllBytes(files[0]);

                Window win = Window.GetWindow(this);
                dynamic d = win?.DataContext;
                if (d != null)
                {
                    d.InstanceInfo.Icon = file;
                    d.InstanceInfo = d.InstanceInfo;
                }
            }
        }

        /// <summary>
		/// ManageIcon - open context menu
		/// </summary>
		private void btnManageIcon_Click(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn != null)
                btn.ContextMenu.IsOpen = true;
        }

        /// <summary>
		/// ManageIcon - Select
		/// </summary>
		private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            if (d != null)
            {
                var window = new IconsList(Commands.Type.SELECT);
                (window.DataContext as D_IconsList).Filters.Module = d.MODULE_TYPE;
                if (window.ShowDialog() == true)
                    if (window.Selected != null)
                    {
                        if (window.Selected.Module == d.MODULE_TYPE)
                        d.InstanceInfo.Icon = window.Selected.ID;
                        d.InstanceInfo = d.InstanceInfo;
                    }
            }
        }

        /// <summary>
		/// ManageIcon - Delete
		/// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            dynamic d = win?.DataContext;
            if (d != null)
            {
                d.InstanceInfo.cIcon = new Tuple<int?, byte[]>(null, null);
                d.InstanceInfo = d.InstanceInfo;
            }
        }
    }
}