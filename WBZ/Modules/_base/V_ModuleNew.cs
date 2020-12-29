﻿using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;

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
			if ((Commands.Type)D.Mode == Commands.Type.DUPLICATE)
            {
				/// groups
				string[] arr = D.MODULE_TYPE.ToLower().Split('_');
				for (int i = 0; i < arr.Length; i++)
					arr[i] = arr[i][0].ToString();
				foreach (M_Group group in SQL.ListInstances<M_Group>(D.MODULE_TYPE, $"{string.Join("", arr)}.id={D.InstanceInfo.ID}"))
					SQL.SetInstance<M_Group>(D.MODULE_TYPE, group, Commands.Type.NEW);
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
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_TYPE, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if ((D.InstanceInfo as dynamic).ID == 0)
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

		/// <summary>
		/// Open module
		/// </summary>
		internal void dgList_Module_MouseDoubleClick<T>(object sender, MouseButtonEventArgs e, string module)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{module}_{Global.UserPermType.SAVE}") ? Commands.Type.EDIT : Commands.Type.PREVIEW;

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
		private void Window_Closed(object sender, EventArgs e)
		{
			if (((Commands.Type)D.Mode).In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_TYPE, (D.InstanceInfo as dynamic).ID);

			if (W.Owner != null)
				W.Owner.Focus();
		}
	}
}
