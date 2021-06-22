using StswExpress;
using System.Windows.Controls;

namespace WBZ.Modules
{
    /// <summary>
    /// Interaction logic for AttributesClassesListGrid.xaml
    /// </summary>
    public partial class AttributesClassesListGrid : DataGrid
    {
        public AttributesClassesListGrid()
        {
            InitializeComponent();
			ExtDataGrid.Load(this, Properties.Settings.Default.panelColor_AttributesClasses);
		}
    }
}
