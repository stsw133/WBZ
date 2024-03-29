﻿using StswExpress;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WBZ.Globals;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_User;

namespace WBZ.Modules.Users
{
    /// <summary>
    /// Interaction logic for UsersNew.xaml
    /// </summary>
    public partial class UsersNew : New
    {
        readonly D_UsersNew D = new D_UsersNew();

        public UsersNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceData = instance;
            D.Mode = mode;

            D.InstanceData.Perms = SQL.GetUserPerms(D.InstanceData.ID);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        internal new void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChkAdmin.Tag = nameof(Admin);
            ChkAdmin.IsChecked = D.InstanceData.Perms.Contains(nameof(Admin));

            var modulesGrids = DckPanPerms.Children.Cast<UniformGrid>().OrderBy(x => (x.Children[0] as Header)?.Text).ToList();
            DckPanPerms.Children.RemoveRange(0, DckPanPerms.Children.Count);
            for (int i = 0; i < modulesGrids.Count; i++)
            {
                foreach (var perm in Enum.GetValues(typeof(Config.PermType)).Cast<Config.PermType>())
                {
                    if (i < 2) break;

                    var check = new ExtCheckBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        IsChecked = D.InstanceData.Perms.Contains($"{modulesGrids[i].Tag}_{perm}"),
                        Tag = $"{modulesGrids[i].Tag}_{perm}"
                    };
                    check.Checked += ChkPerms_Checked;
                    check.Unchecked += ChkPerms_Unchecked;
                    modulesGrids[i].Children.Add(check);
                }
                DckPanPerms.Children.Add(modulesGrids[i]);
            }
        }

        /// <summary>
        /// Add perm
        /// </summary>
        private void ChkPerms_Checked(object sender, RoutedEventArgs e)
        {
            var perm = (sender as CheckBox).Tag.ToString();
            if (!D.InstanceData.Perms.Contains(perm))
                D.InstanceData.Perms.Add(perm);
        }

        /// <summary>
        /// Remove perm
        /// </summary>
        private void ChkPerms_Unchecked(object sender, RoutedEventArgs e)
        {
            var perm = (sender as CheckBox).Tag.ToString();
            if (D.InstanceData.Perms.Contains(perm))
                D.InstanceData.Perms.Remove(perm);
        }

        /// <summary>
        /// PasswordChanged
        /// </summary>
        private void PwdBoxNewpass_PasswordChanged(object sender, RoutedEventArgs e) =>
            D.InstanceData.Newpass = (sender as PasswordBox).Password;

        /// <summary>
        /// Validation
        /// </summary>
        internal override bool CheckDataValidation()
        {
            if (string.IsNullOrEmpty(D.InstanceData.Login))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano nazwy użytkownika!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceData.Email))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano adresu e-mail!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
