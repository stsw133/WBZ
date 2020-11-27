using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WBZ.Controls;
using WBZ.Models;
using WBZ.Modules.Articles;
using WBZ.Modules.AttributesClasses;
using WBZ.Modules.Companies;
using WBZ.Modules.Distributions;
using WBZ.Modules.Documents;
using WBZ.Modules.Employees;
using WBZ.Modules.Families;
using WBZ.Modules.Stores;
using WBZ.Modules.Users;

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
			if (SQL.CountInstances(obj.Module, $"{string.Join("", (obj.Module.Split('_').AsQueryable().Cast<string>() as IQueryable<string>).Select(str => str.Substring(0, 1)))}.id={obj.Instance}") == 0)
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
			var moduleName = string.Join("", moduleNames);
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
				/// COMPANIES
				case Global.Module.COMPANIES:
					window = new CompaniesNew(SQL.GetInstance<M_Company>(obj.Module, obj.Instance), mode);
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
            try
            {
                var process = new Process();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/pl_manual.pdf";
                process.StartInfo.FileName = new Uri(path, UriKind.RelativeOrAbsolute).LocalPath;
                process.Start();
            }
            catch (Exception ex)
            {
                new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, "Błąd otwierania poradnika: " + ex.Message) { Owner = owner }.ShowDialog();
            }
        }

		/// <summary>
		/// Load image from byte[] to BitmapImage
		/// </summary>
		/// <param name="imageData"></param>
		/// <returns></returns>
		internal static BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0) return null;
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData))
			{
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();
			return image;
		}
	}
}
