using StswExpress;
using System.Windows;
using WBZ.Models;
using WBZ.Modules.Articles;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Contractors;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Icons;
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
		/*
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
		*/
		/// <summary>
		/// Open instance window
		/// </summary>
		internal static void OpenInstanceWindow<T>(Window owner, T instance, Commands.Type mode)
		{
			var mm = instance as IMM;

			if (mm.InstanceID == 0)
				return;

			if (SQL.CountInstances(mm.Module, $"{mm.Module.Tag}.id={mm.InstanceID}") == 0)
				return;

			if (!(mode == Commands.Type.EDIT && Config.User.Perms.Contains($"{mm.Module.Name}_{Config.PermType.SAVE}")))
				mode = Commands.Type.PREVIEW;
			if (!Config.User.Perms.Contains($"{mm.Module.Name}_{Config.PermType.PREVIEW}") && !Config.User.Perms.Contains($"{mm.Module.Name}_{Config.PermType.SAVE}"))
				return;

			Window window;
			
			//OpenWindow(owner, $"WBZ.Modules.{module.Name}.{module.Name}New", false, SQL.GetInstance<new A>(module, mm.InstanceID), mode);
			
			switch (mm.Module.Name)
			{
				/// ARTICLES
				case nameof(Modules.Articles):
					window = new ArticlesNew(SQL.GetInstance<M_Article>(mm.Module, mm.InstanceID), mode);
					break;
				/// ATTRIBUTES_CLASSES
				case nameof(Modules.AttributesClasses):
					window = new AttributesClassesNew(SQL.GetInstance<M_AttributeClass>(mm.Module, mm.InstanceID), mode);
					break;
				/// CONTRACTORS
				case nameof(Modules.Contractors):
					window = new ContractorsNew(SQL.GetInstance<M_Contractor>(mm.Module, mm.InstanceID), mode);
					break;
				/// DISTRIBUTIONS
				case nameof(Modules.Distributions):
					window = new DistributionsNew(SQL.GetInstance<M_Distribution>(mm.Module, mm.InstanceID), mode);
					break;
				/// DOCUMENTS
				case nameof(Modules.Documents):
					window = new DocumentsNew(SQL.GetInstance<M_Document>(mm.Module, mm.InstanceID), mode);
					break;
				/// EMPLOYEES
				case nameof(Modules.Employees):
					window = new EmployeesNew(SQL.GetInstance<M_Employee>(mm.Module, mm.InstanceID), mode);
					break;
				/// FAMILIES
				case nameof(Modules.Families):
					window = new FamiliesNew(SQL.GetInstance<M_Family>(mm.Module, mm.InstanceID), mode);
					break;
				/// ICONS
				case nameof(Modules.Icons):
					window = new IconsNew(SQL.GetInstance<M_Icon>(mm.Module, mm.InstanceID), mode);
					break;
				/// STORES
				case nameof(Modules.Stores):
					window = new StoresNew(SQL.GetInstance<M_Store>(mm.Module, mm.InstanceID), mode);
					break;
				/// USERS
				case nameof(Modules.Users):
					window = new UsersNew(SQL.GetInstance<M_User>(mm.Module, mm.InstanceID), mode);
					break;
				/// VEHICLES
				case nameof(Modules.Vehicles):
					window = new VehiclesNew(SQL.GetInstance<M_Vehicle>(mm.Module, mm.InstanceID), mode);
					break;
				default:
					return;
			}
			window.Owner = owner;
			window.Show();
		}
	}
}
