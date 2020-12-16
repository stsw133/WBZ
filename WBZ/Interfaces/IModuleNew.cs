using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;

namespace WBZ.Interfaces
{
	interface IModuleNew
	{
		void Init();
		void Window_Loaded(object sender, RoutedEventArgs e);
		bool CheckDataValidation();
		void btnSave_Click(object sender, MouseButtonEventArgs e);
		void btnRefresh_Click(object sender, MouseButtonEventArgs e);
		void btnClose_Click(object sender, MouseButtonEventArgs e);
		void Window_Closed(object sender, EventArgs e);
	}

    public class ModuleNew<MODULE_MODEL> : Window, IModuleNew where MODULE_MODEL : class, new()
    {
		dynamic W,  D;
        string FullName, HalfName;

		/// <summary>
		/// Init
		/// </summary>
		public void Init()
		{
			W = GetWindow(this);
			D = W.DataContext;
			FullName = W.GetType().FullName;
			HalfName = FullName.Substring(0, FullName.Length - 4);
		}

		/// <summary>
		/// Loaded
		/// </summary>
		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE))
				(D.InstanceInfo as dynamic).ID = SQL.NewInstanceID(D.MODULE_TYPE);
		}

        /// <summary>
		/// Validation
		/// </summary>
		public bool CheckDataValidation()
        {
            bool result = true;

            return result;
        }

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		public void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_TYPE, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		public void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if ((D.InstanceInfo as dynamic).ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		public void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Open module
		/// </summary>
		public void dgList_Module_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e, string module)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{module}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<T>();
				foreach (T instance in selectedInstances)
				{
					var window = Activator.CreateInstance(Type.GetType($"WBZ.Modules.{module.Replace("_",string.Empty)}.{module.Replace("_", string.Empty)}New"), instance, perm) as Window;
					window.Show();
				}
			}
		}
		
		/// <summary>
		/// Closed
		/// </summary>
		public void Window_Closed(object sender, EventArgs e)
		{
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_TYPE, (D.InstanceInfo as dynamic).ID);

			if (W.Owner != null)
				W.Owner.Focus();
		}
	}
}
