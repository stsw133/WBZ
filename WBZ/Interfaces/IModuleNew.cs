using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Helpers;

namespace WBZ.Interfaces
{
	interface IModuleNew
	{
		void Window_Loaded(object sender, RoutedEventArgs e);
		bool CheckDataValidation();
		void btnSave_Click(object sender, MouseButtonEventArgs e);
		void btnRefresh_Click(object sender, MouseButtonEventArgs e);
		void btnClose_Click(object sender, MouseButtonEventArgs e);
		void Window_Closed(object sender, EventArgs e);
	}

    public class ModuleNew<MODULE_MODEL> : Window, IModuleNew where MODULE_MODEL : class, new()
    {
        dynamic W, D;
        string FullName, HalfName;
        string MODULE_TYPE;

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            W = GetWindow(this);
            D = W.DataContext;

            FullName = W.GetType().FullName;
            HalfName = FullName.Substring(0, FullName.Length - 4);

            MODULE_TYPE = D.MODULE_TYPE;

            switch (MODULE_TYPE)
            {
                ///ARTICLES
                case Global.Module.ARTICLES:
                    D.InstanceInfo.Measures = SQL.GetArticleMeasures(D.InstanceInfo.ID);
                    if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE))
                    {
                        D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_TYPE);
                        foreach (DataRow row in D.InstanceInfo.Measures.Rows)
                            row.SetAdded();
                    }
                    break;
            }
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
			if (D.InstanceInfo.ID == 0)
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
		/*
		/// <summary>
		/// Open: Store
		/// </summary>
		private void dgList_Stores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.STORES}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Store>();
				foreach (C_Store instance in selectedInstances)
				{
					var window = new StoresNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Document>();
				foreach (C_Document instance in selectedInstances)
				{
					var window = new DocumentsNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Distribution
		/// </summary>
		private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.DISTRIBUTIONS}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Distribution>();
				foreach (C_Distribution instance in selectedInstances)
				{
					var window = new DistributionsNew(instance, perm);
					window.Show();
				}
			}
		}
		*/
		/// <summary>
		/// Closed
		/// </summary>
		public void Window_Closed(object sender, EventArgs e)
		{
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_TYPE, D.InstanceInfo.ID);
		}
	}
}
