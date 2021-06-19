using StswExpress;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Controls;
using WBZ.Models;
using WBZ.Modules.Icons;

namespace WBZ.Modules._shared
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

        /// <summary>
        /// HasIcon
        /// </summary>
        public bool HasIcon
        {
            get => (bool)GetValue(pHasIcon);
            set => SetValue(pHasIcon, value);
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
                    d.InstanceData.IconContent = file;
                    d.InstanceData = d.InstanceData;
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
            var d = win?.DataContext as D_IconsNew;

            if (d != null)
            {
                var window = new IconsList(Commands.Type.SELECT);
                (window.DataContext as D_IconsList).Filter.Module = d.Module;   //TODO - ogarnąć coś z filtrem w kolumnie modułu w oknie listy ikon
                if (window.ShowDialog() == true)
                    if (window.Selected != null)
                    {
                        if (window.Selected.Module.Alias.In(string.Empty, d.Module.Alias))
                        {
                            d.InstanceData.Icon = window.Selected.ID;
                            d.InstanceData.IconContent = window.Selected.File;
                            d.InstanceData = d.InstanceData;
                        }
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
                d.InstanceData.IconContent = null;
                d.InstanceData.Icon = 0;
                d.InstanceData = d.InstanceData;
            }
        }
    }
}