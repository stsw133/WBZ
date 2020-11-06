using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using WBZ.Controls;
using WBZ.Models;
using WBZ.Modules.Users;

namespace WBZ.Helpers
{
    internal static class Functions
    {
        /// <summary>
        /// Open window
        /// </summary>
        internal static void OpenWindow(string fullname, Window owner = null, bool dialog = false)
        {
            var window = Activator.CreateInstance(Type.GetType(fullname)) as Window;

            if (owner != null)
                window.Owner = owner;

            if (dialog)
                window.ShowDialog();
            else
                window.Show();
        }

        /// <summary>
        /// Open window - Help
        /// </summary>
        internal static void OpenWindow_Help(Window owner)
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
        /// Open window - Settings
        /// </summary>
        internal static void OpenWindow_Settings(Window owner)
        {
            OpenWindow(typeof(Modules.Settings).FullName, owner, true);
        }

        /// <summary>
        /// Open window - Module
        /// </summary>
        internal static bool? OpenWindow_Module(Window owner, string module, Commands.Type type, object model = null)
        {
            Window window;
            bool selectingMode = type == Commands.Type.SELECTING;

            switch (module)
            {
                ///USERS
                case Global.Module.USERS:
                    ///LIST
                    if (type.In(Commands.Type.LIST, Commands.Type.SELECTING))
                        window = new UsersList(selectingMode);
                    ///NEW
                    else if (type.In(Commands.Type.DUPLICATE, Commands.Type.EDIT, Commands.Type.NEW, Commands.Type.PREVIEW))
                        window = new UsersNew(model != null ? (C_User)model : new C_User(), type);
                    else return null;
                    break;
                default:
                    return null;
            }
            window.Owner = owner;
            if (selectingMode)
                return window.ShowDialog();
            else
                window.Show();

            return null;
        }
    }
}
