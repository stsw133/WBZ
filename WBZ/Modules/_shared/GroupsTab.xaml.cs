using StswExpress;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using WBZ.Modules._submodules;

namespace WBZ.Modules._shared
{
	/// <summary>
	/// Interaction logic for GroupsTab.xaml
	/// </summary>
	public partial class GroupsTab : UserControl
	{
		readonly D_GroupsTab D = new D_GroupsTab();

		private MV Module;
		private int InstanceID;

		public GroupsTab()
		{
			InitializeComponent();
			DataContext = D;
		}

		/// <summary>
		/// Loaded
		/// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
			try
			{
				var win = Window.GetWindow(this);
				var d = win?.DataContext as D_ModuleNew<dynamic>;

				if (d != null)
				{
					Module = d.Module;
					InstanceID = (d.InstanceData as M).ID;
				}
				if (InstanceID != 0 && D.InstanceGroups == null)
					D.InstanceGroups = SQL.ListInstances<M_Group>(D.ModuleGroups, $"{D.ModuleGroups.Alias}.module_alias='{Module}' and {D.ModuleGroups.Alias}.instance_id={InstanceID}");
			}
			catch { }
		}

		/// <summary>
		/// Add
		/// </summary>
		private void btnGroupAdd_Click(object sender, RoutedEventArgs e)
		{
			var window = new GroupsList(Module, Commands.Type.SELECT);
			window.Owner = Window.GetWindow(this);
			if (window.ShowDialog() == true)
            {
				var group = window.Selected;
				group.OwnerID = group.ID;
				group.ID = SQL.NewInstanceID(Config.GetModule(nameof(_submodules.Groups)));
				group.InstanceID = InstanceID;
                SQL.SetInstance(Config.GetModule(nameof(_submodules.Groups)), group, Commands.Type.NEW);
                D.InstanceGroups = SQL.ListInstances<M_Group>(D.ModuleGroups, $"{D.ModuleGroups.Alias}.module_alias='{Module.Alias}' and {D.ModuleGroups.Alias}.instance_id={InstanceID}");
			}
		}

		/// <summary>
		/// Remove
		/// </summary>
		private void btnGroupRemove_Click(object sender, RoutedEventArgs e)
		{
			var selectedInstances = dgListGroups.SelectedItems.Cast<M_Group>();
			if (selectedInstances.Count() > 0)
			{
				foreach (var instance in selectedInstances)
                    SQL.DeleteInstance(Config.GetModule(nameof(_submodules.Groups)), instance.ID, instance.Name);
                D.InstanceGroups = SQL.ListInstances<M_Group>(Config.GetModule(nameof(_submodules.Groups)), $"{D.ModuleGroups.Alias}.module_alias='{Module.Alias}' and {D.ModuleGroups.Alias}.instance_id={InstanceID}");
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
		public MV ModuleGroups = Config.GetModule(nameof(_submodules.Groups));

		/// Groups
		private List<M_Group> instanceGroups;
		public List<M_Group> InstanceGroups
		{
			get => instanceGroups;
			set => SetField(ref instanceGroups, value, () => InstanceGroups);
		}
	}
}
