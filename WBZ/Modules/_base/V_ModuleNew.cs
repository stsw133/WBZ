using StswExpress.Globals;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WBZ.Modules._base
{
    public class ModuleNew<MODULE_MODEL> : Window where MODULE_MODEL : class, new()
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
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			int newID = (D.InstanceInfo as dynamic).ID;
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE))
			{
				newID = SQL.NewInstanceID(D.MODULE_TYPE);
			}
			/*
			if ((Commands.Type)D.Mode == Commands.Type.DUPLICATE)
            {
				/// groups
				foreach (M_Group group in SQL.ListInstances<M_Group>(D.MODULE_TYPE, $"{Global.GetModuleAlias(D.MODULE_TYPE)}.id={D.InstanceInfo.ID}"))
				{
					group.ID = newID;
					SQL.SetInstance<M_Group>(Global.Module.GROUPS, group, Commands.Type.NEW);
				}
				/// contacts
				DataTable contacts = SQL.ListContacts(D.MODULE_TYPE, D.InstanceInfo.ID);
				foreach (DataRow contact in contacts.Rows)
					contact.SetAdded();
				SQL.UpdateContacts(D.MODULE_TYPE, D.InstanceInfo.ID, contacts);
				/// attributes
				foreach (M_Attribute attribute in SQL.ListAttributes(D.MODULE_TYPE, D.InstanceInfo.ID))
				{
					attribute.Instance = D.InstanceInfo.ID;
					SQL.UpdateAttribute(attribute);
				}
            }
			*/
			(D.InstanceInfo as dynamic).ID = newID;
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
		internal void cmdSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			try
			{
				if (D.EditingMode)
					e.CanExecute = true;
				else
					e.CanExecute = false;
			}
			catch { }
		}
		internal void cmdSave_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_TYPE, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		internal void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if ((D.InstanceInfo as dynamic).ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Help
		/// </summary>
		internal void cmdHelp_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Globals.Functions.OpenHelp(this);
		}

		/// <summary>
		/// Close
		/// </summary>
		internal void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			W.Close();
		}

		/// <summary>
		/// Open module
		/// </summary>
		internal void dgList_Module_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e, string module)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Globals.Global.User.Perms.Contains($"{module}_{Globals.Global.UserPermType.SAVE}") ? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<T>();
				foreach (T instance in selectedInstances)
				{
					var winNames = module.Split('_');
					for (int i = 0; i < winNames.Length; i++)
						winNames[i] = winNames[i].First().ToString().ToUpper() + string.Join("", winNames[i].Skip(1));
					var window = Activator.CreateInstance(Type.GetType($"WBZ.Modules.{string.Join("",winNames)}.{string.Join("", winNames)}New"), instance, perm) as Window;
					window.Show();
				}
			}
		}

		/// <summary>
		/// Closed
		/// </summary>
		private void Window_Closed(object sender, EventArgs e)
		{
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_TYPE, (D.InstanceInfo as dynamic).ID);

			if (W.Owner != null)
				W.Owner.Focus();
		}
	}
}
