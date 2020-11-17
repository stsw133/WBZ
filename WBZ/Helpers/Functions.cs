using System;
using System.Diagnostics;
using System.Windows;
using WBZ.Controls;

namespace WBZ.Helpers
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
    }
}
