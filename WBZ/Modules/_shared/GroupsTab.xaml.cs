using StswExpress;
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
		D_GroupsTab D = new D_GroupsTab();
		private string Module;
		private int ID;

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
				Window win = Window.GetWindow(this);
				dynamic d = win?.DataContext;
				if (d != null)
				{
					Module = (string)d.Module;
					ID = (int)d.InstanceData.ID;
				}
				if (ID != 0 && D.InstanceGroups == null)
					D.InstanceGroups = SQL.ListInstances<M_Group>(Config.GetModule(nameof(Modules._submodules.Groups)), $"g.module='{Module}' and g.instance={ID}");
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
				group.Owner = group.ID;
				group.ID = SQL.NewInstanceID(Config.GetModule(nameof(Modules._submodules.Groups)));
				group.Instance = ID;
                SQL.SetInstance(Config.GetModule(nameof(Modules._submodules.Groups)), group, Commands.Type.NEW);
                D.InstanceGroups = SQL.ListInstances<M_Group>(Config.GetModule(nameof(Modules._submodules.Groups)), $"g.module='{Module}' and g.instance={ID}");
			}
		}

		/// <summary>
		/// Remove
		/// </summary>
		private void btnGroupRemove_Click(object sender, RoutedEventArgs e)
		{
			var selectedInstances = dgList.SelectedItems.Cast<M_Group>();
			if (selectedInstances.Count() > 0)
			{
				foreach (var instance in selectedInstances)
                    SQL.DeleteInstance(Config.GetModule(nameof(Modules._submodules.Groups)), instance.ID, instance.Name);
                D.InstanceGroups = SQL.ListInstances<M_Group>(Config.GetModule(nameof(Modules._submodules.Groups)), $"g.module='{Module}' and g.instance={ID}");
			}
		}

		/// <summary>
		/// Select as main
		/// </summary>
		private void btnGroupSelectAsMain_Click(object sender, RoutedEventArgs e)
		{

		}
	}

	/// <summary>
	/// DataContext
	/// </summary>
	class D_GroupsTab : D
	{
		/// Groups
		private List<M_Group> instanceGroups;
		public List<M_Group> InstanceGroups
		{
			get => instanceGroups;
			set => SetField(ref instanceGroups, value, () => InstanceGroups);
		}
	}
}
