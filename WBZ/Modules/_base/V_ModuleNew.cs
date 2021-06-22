using StswExpress;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules._base
{
    public abstract class ModuleNew<MODULE_MODEL> : Window where MODULE_MODEL : class, new()
    {
		private Window W;
		private D_ModuleNew<MODULE_MODEL> D;
        string Namespace;
		
		/// <summary>
		/// Init
		/// </summary>
		public void Init()
		{
			W = GetWindow(this);
			D = W.DataContext as D_ModuleNew<MODULE_MODEL>;
			Namespace = W.GetType().FullName[0..^4];
		}

		/// <summary>
		/// Loaded
		/// </summary>
		internal void Window_Loaded(object sender, RoutedEventArgs e)
		{
			int newID = (D.InstanceData as M).ID;
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
				newID = SQL.NewInstanceID(D.Module);
			/*
			if ((Commands.Type)D.Mode == Commands.Type.DUPLICATE)
            {
				/// groups
				foreach (M_Group group in SQL.ListInstances<M_Group>(D.Module, $"{Global.GetModuleAlias(D.Module)}.id={D.InstanceData.ID}"))
				{
					group.ID = newID;
					SQL.SetInstance<M_Group>(M_Module.Module.GROUPS, group, Commands.Type.NEW);
				}
				/// contacts
				DataTable contacts = SQL.ListContacts(D.Module, D.InstanceData.ID);
				foreach (DataRow contact in contacts.Rows)
					contact.SetAdded();
				SQL.UpdateContacts(D.Module, D.InstanceData.ID, contacts);
				/// attributes
				foreach (M_Attribute attribute in SQL.ListAttributes(D.Module, D.InstanceData.ID))
				{
					attribute.Instance = D.InstanceData.ID;
					SQL.UpdateAttribute(attribute);
				}
            }
			*/
			(D.InstanceData as M).ID = newID;
		}

		/// <summary>
		/// Validation
		/// </summary>
		internal virtual bool CheckDataValidation()
		{
			//TODO - dynamicznie po FontWeight->Bold w zakładce głównej (???)
			return true;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		internal void cmdSave_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.Module, D.InstanceData, D.Mode))
			{
				if (Owner != null)
					DialogResult = true;
                else Close();
			}
		}
		internal void cmdSave_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
			e.CanExecute = D?.Mode != Commands.Type.PREVIEW;

		/// <summary>
		/// Refresh
		/// </summary>
		internal void cmdRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if ((D.InstanceData as M).ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		internal void cmdClose_Executed(object sender, ExecutedRoutedEventArgs e) => W.Close();

		/// <summary>
		/// Open module
		/// </summary>
		internal void dgSourceList_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e, MV module)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var perm = Config.User.Perms.Contains($"{module.Name}_{Config.PermType.SAVE}") ? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as System.Windows.Controls.DataGrid).SelectedItems.Cast<T>();
				foreach (T instance in selectedInstances)
					(Activator.CreateInstance(Type.GetType($"WBZ.Modules.{module.Name}.{module.Name}New"), instance, perm) as Window).Show();
			}
		}

		/// <summary>
		/// Closed
		/// </summary>
		internal void Window_Closed(object sender, EventArgs e)
		{
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.Module, (D.InstanceData as M).ID);

			Properties.Settings.Default.Save();
			if (W.Owner != null)
				W.Owner.Focus();
		}
	}
}
