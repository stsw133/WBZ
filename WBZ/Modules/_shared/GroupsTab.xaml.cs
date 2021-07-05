using StswExpress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._submodules;

namespace WBZ.Modules._shared
{
    /// <summary>
    /// Interaction logic for GroupsTab.xaml
    /// </summary>
    public partial class GroupsTab : UserControl
    {
        readonly D_GroupsTab D = new D_GroupsTab();

        public GroupsTab()
        {
            InitializeComponent();
            //DataContext = D;
        }

        /// <summary>
        /// Module
        /// </summary>
        public static readonly DependencyProperty ModuleProperty
            = DependencyProperty.Register(
                  nameof(Module),
                  typeof(MV),
                  typeof(GroupsTab),
                  new PropertyMetadata(default(MV))
              );
        public MV Module
        {
            get => (MV)GetValue(ModuleProperty);
            set => SetValue(ModuleProperty, value);
        }

        /// <summary>
        /// InstanceID
        /// </summary>
        public static readonly DependencyProperty InstanceIDProperty
            = DependencyProperty.Register(
                  nameof(InstanceID),
                  typeof(int),
                  typeof(GroupsTab),
                  new PropertyMetadata(default(int))
              );
        public int InstanceID
        {
            get => (int)GetValue(InstanceIDProperty);
            set => SetValue(InstanceIDProperty, value);
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InstanceID > 0 && D.InstanceGroups == null)
                    D.InstanceGroups = SQL.ListInstances<M_Group>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
                DataContext = D;
                Loaded -= UserControl_Loaded;
            }
            catch (Exception ex)
            {
                SQL.Error("Błąd inicjalizacji zakładki grup", ex, Module, InstanceID);
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        private void BtnGroupAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new GroupsList(Module, Commands.Type.SELECT);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                var group = new M_Group()
                {
                    OwnerID = window.Selected.ID,
                    ID = SQL.NewInstanceID(D.Module),
                    Module = Module,
                    InstanceID = InstanceID
                };
                SQL.SetInstance(Config.GetModule(nameof(_submodules.Groups)), group, Commands.Type.NEW);
                D.InstanceGroups = SQL.ListInstances<M_Group>(D.Module, $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        private void BtnGroupRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectedInstances = DtgListGroups.SelectedItems.Cast<M_Group>();
            if (selectedInstances.Count() > 0)
            {
                foreach (var instance in selectedInstances)
                    SQL.DeleteInstance(Config.GetModule(nameof(_submodules.Groups)), instance.ID, instance.Name);
                D.InstanceGroups = SQL.ListInstances<M_Group>(Config.GetModule(nameof(_submodules.Groups)), $"{D.Module.Alias}.module_alias='{Module.Alias}' and {D.Module.Alias}.instance_id={InstanceID}");
            }
        }

        /// <summary>
        /// Select as main
        /// </summary>
        private void btnGroupSelectAsMain_Click(object sender, RoutedEventArgs e)
        {
            //TODO - grupy domyślne
        }
    }

    /// <summary>
    /// DataContext
    /// </summary>
    class D_GroupsTab : D
    {
        /// Module
        public MV Module = Config.GetModule(nameof(_submodules.Groups));

        /// Groups
        private List<M_Group> instanceGroups;
        public List<M_Group> InstanceGroups
        {
            get => instanceGroups;
            set => SetField(ref instanceGroups, value, () => InstanceGroups);
        }
    }
}
