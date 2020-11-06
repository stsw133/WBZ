using System;
using System.Windows;
using System.Windows.Input;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Models.C_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesNew.xaml
	/// </summary>
	public partial class AttributesClassesNew : Window
	{
        D_AttributesClassesNew D = new D_AttributesClassesNew();

        public AttributesClassesNew(MODULE_CLASS instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;

            D.InstanceInfo = instance;
            D.Mode = mode;

            if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
                D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
        }

        /// <summary>
		/// Validation
		/// </summary>
        private bool CheckDataValidation()
        {
            bool result = true;

            return result;
        }

        /// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (!CheckDataValidation())
                return;

            if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
                Close();
        }

        /// <summary>
		/// Refresh
		/// </summary>
        private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            if (D.InstanceInfo.ID == 0)
                return;
            //TODO - dorobić odświeżanie zmienionych danych
        }

        /// <summary>
		/// Close
		/// </summary>
        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
                SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
        }
    }
}
