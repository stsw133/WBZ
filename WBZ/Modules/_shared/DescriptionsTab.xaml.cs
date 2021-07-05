using System.Windows;
using System.Windows.Controls;
using WBZ.Models;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for DescriptionsTab.xaml
    /// </summary>
    public partial class DescriptionsTab : UserControl
    {
        public DescriptionsTab()
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
                  typeof(DescriptionsTab),
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
                  typeof(DescriptionsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }
    }
}