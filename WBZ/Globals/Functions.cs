using StswExpress.Globals;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WBZ.Controls;
using WBZ.Models;
using WBZ.Modules.Articles;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Contractors;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;
using WBZ.Modules.Vehicles;

namespace WBZ.Globals
{
    internal static class Functions
    {
        /// <summary>
        /// Open window
        /// </summary>
        internal static void OpenWindow(Window owner, string fullname, bool dialog, params object[] args)
        {
            var window = Activator.CreateInstance(Type.GetType(fullname), args) as Window;

            if (owner != null)
                window.Owner = owner;

            if (dialog)
                window.ShowDialog();
            else
                window.Show();
        }

		/// <summary>
		/// Open instance window
		/// </summary>
		internal static void OpenInstanceWindow(Window owner, dynamic obj, Commands.Type mode)
		{
			if (obj.Instance == 0)
				return;
			if (SQL.CountInstances(obj.Module, $"{string.Join(string.Empty, ((obj.Module.Split('_') as string[]).AsQueryable().Cast<string>()).Select(str => str.Substring(0, 1)))}.id={obj.Instance}") == 0)
				return;

			if (!(mode == Commands.Type.EDIT && Global.User.Perms.Contains($"{obj.Module}_{Global.UserPermType.SAVE}")))
				mode = Commands.Type.PREVIEW;
			if (!Global.User.Perms.Contains($"{obj.Module}_{Global.UserPermType.PREVIEW}") && !Global.User.Perms.Contains($"{obj.Module}_{Global.UserPermType.SAVE}"))
				return;
			Window window;
			/*
			var moduleNames = (obj.Module as string).Split('_');
			for (int i = 0; i < moduleNames.Length; i++)
				moduleNames[i] = char.ToUpper(moduleNames[i][0]) + moduleNames[i].Substring(1);
			var moduleName = string.Join(string.Empty, moduleNames);
			OpenWindow(owner, $"WBZ.Modules.{moduleName}.{moduleName}New", false, SQL.GetInstance<MODULE_MODEL>(obj.Module, obj.Instance), mode);
			*/
			switch (obj.Module)
			{
				/// ARTICLES
				case Global.Module.ARTICLES:
					window = new ArticlesNew(SQL.GetInstance<M_Article>(obj.Module, obj.Instance), mode);
					break;
				/// ATTRIBUTES_CLASSES
				case Global.Module.ATTRIBUTES_CLASSES:
					window = new AttributesClassesNew(SQL.GetInstance<M_AttributeClass>(obj.Module, obj.Instance), mode);
					break;
				/// CONTRACTORS
				case Global.Module.CONTRACTORS:
					window = new ContractorsNew(SQL.GetInstance<M_Contractor>(obj.Module, obj.Instance), mode);
					break;
				/// DISTRIBUTIONS
				case Global.Module.DISTRIBUTIONS:
					window = new DistributionsNew(SQL.GetInstance<M_Distribution>(obj.Module, obj.Instance), mode);
					break;
				/// DOCUMENTS
				case Global.Module.DOCUMENTS:
					window = new DocumentsNew(SQL.GetInstance<M_Document>(obj.Module, obj.Instance), mode);
					break;
				/// EMPLOYEES
				case Global.Module.EMPLOYEES:
					window = new EmployeesNew(SQL.GetInstance<M_Employee>(obj.Module, obj.Instance), mode);
					break;
				/// FAMILIES
				case Global.Module.FAMILIES:
					window = new FamiliesNew(SQL.GetInstance<M_Family>(obj.Module, obj.Instance), mode);
					break;
				/// STORES
				case Global.Module.STORES:
					window = new StoresNew(SQL.GetInstance<M_Store>(obj.Module, obj.Instance), mode);
					break;
				/// USERS
				case Global.Module.USERS:
					window = new UsersNew(SQL.GetInstance<M_User>(obj.Module, obj.Instance), mode);
					break;
				/// VEHICLES
				case Global.Module.VEHICLES:
					window = new VehiclesNew(SQL.GetInstance<M_Vehicle>(obj.Module, obj.Instance), mode);
					break;
				default:
					return;
			}
			window.Owner = owner;
			window.Show();
		}

		/// <summary>
		/// Open window - Help
		/// </summary>
		internal static void OpenHelp(Window owner)
        {
			//TODO - przerobienie poradnika na okno pomocy w wersji 1.2.0 (?)
			//TODO - odpalenie poradnika z dowolnego miejsca w aplikacji (App.xaml)
            try
            {
                var process = new Process();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/pl_manual.pdf";
                process.StartInfo.FileName = new Uri(path, UriKind.RelativeOrAbsolute).LocalPath;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "open";
                process.Start();
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd otwierania poradnika: " + ex.Message) { Owner = owner }.ShowDialog();
            }
        }
	}
}
