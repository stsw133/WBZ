using StswExpress;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Modules._base;
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
        /// Module
        /// </summary>
        public static readonly DependencyProperty ModuleProperty
            = DependencyProperty.Register(
                  nameof(Module),
                  typeof(MV),
                  typeof(AdditionalsTab),
                  new PropertyMetadata(default(MV))
              );
        public MV Module
        {
            get => (MV)GetValue(ModuleProperty);
            set => SetValue(ModuleProperty, value);
        }

        /// <summary>
        /// InstanceID
        /// </summary>
        public static readonly DependencyProperty InstanceIDProperty
            = DependencyProperty.Register(
                  nameof(InstanceID),
                  typeof(int),
                  typeof(AdditionalsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }

        /// <summary>
        /// HasIcon
        /// </summary>
        public static readonly DependencyProperty HasIconProperty
            = DependencyProperty.Register(
                  nameof(HasIcon),
                  typeof(bool),
                  typeof(AdditionalsTab),
                  new PropertyMetadata(true)
              );
        public bool HasIcon
        {
            get => (bool)GetValue(HasIconProperty);
            set => SetValue(HasIconProperty, value);
        }

        /// <summary>
        /// ManageIcon - drop image
        /// </summary>
        private void btnManageIcon_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = File.ReadAllBytes(files[0]);

                var win = Window.GetWindow(this);
                var d = win?.DataContext as dynamic;

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
        private void btnManageIcon_Click(object sender, MouseButtonEventArgs e) => Fn.OpenContextMenu(sender);

        /// <summary>
		/// ManageIcon - Select
		/// </summary>
		private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            var d = win?.DataContext as dynamic;

            if (d != null)
            {
                var window = new IconsList(Commands.Type.SELECT);
                //TODO - ogarnąć coś z filtrem w kolumnie modułu w oknie listy ikon
                //(window.DataContext as D_IconsList).Filter.Module = d.Module;
                if (window.ShowDialog() == true)
                    if (window.Selected != null && window.Selected.Module.Alias.In(string.Empty, (string)d.Module.Alias))
                    {
                        d.InstanceData.IconID = window.Selected.ID;
                        d.InstanceData.IconContent = window.Selected.Content;
                        d.InstanceData = d.InstanceData;
                    }
            }
        }

        /// <summary>
		/// ManageIcon - Delete
		/// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            var d = win?.DataContext as dynamic;

            if (d != null)
            {
                d.InstanceData.IconContent = null;
                d.InstanceData.IconID = 0;
                d.InstanceData = d.InstanceData;
            }
        }
    }
}