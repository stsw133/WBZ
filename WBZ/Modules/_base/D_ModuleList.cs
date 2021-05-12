using StswExpress.Base;
using StswExpress.Globals;
using StswExpress.Translate;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WBZ.Modules._base
{
	abstract class D_ModuleList<MODULE_MODEL> : D where MODULE_MODEL : class, new()
	{
		/// Module
		public string Module => GetType().Name.ToLower()[2..^4];
		public string Title => TM.Tr(Module.Capitalize() + Mode.ToString().Capitalize());

		/// Mode
		public Commands.Type Mode { get; set; }
		public string ModeIcon
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "/Resources/icon32_list.ico";
				else if	(Mode == Commands.Type.SELECT)	return "/Resources/icon32_select.ico";
				else									return null;
			}
		}

		/// Selected tab
		private int selectedTab = 0;
		public int SelectedTab
        {
			get => selectedTab;
			set
			{
				SetField(ref selectedTab, value, () => SelectedTab);
				InstancesList = instancesLists[value];
			}
		}

		/// Instances lists
		private ObservableCollection<ObservableCollection<MODULE_MODEL>> instancesLists;
		public ObservableCollection<ObservableCollection<MODULE_MODEL>> InstancesLists
		{
			get => instancesLists;
			set => SetField(ref instancesLists, value, () => InstancesLists);
		}

		/// Instances list
		private ObservableCollection<MODULE_MODEL> instancesList = new ObservableCollection<MODULE_MODEL>();
		public ObservableCollection<MODULE_MODEL> InstancesList
		{
			get => instancesList;
			set => SetField(ref instancesList, value, () => InstancesList);
		}

		/// Sorting
		public StringCollection Sorting
		{
			get => (StringCollection)Properties.Settings.Default[$"sorting_{Module.Capitalize()}List"];
			set => Properties.Settings.Default[$"sorting_{Module.Capitalize()}List"] = value;
		}

		/// SQL filter
		public string FilterSQL { get; set; }

		/// Filters instance
		private MODULE_MODEL filters = new MODULE_MODEL();
		public MODULE_MODEL Filters
		{
			get => filters;
			set => SetField(ref filters, value, () => Filters);
		}

		/// Filters list
		private ObservableCollection<string> filtersList = new ObservableCollection<string>();
		public ObservableCollection<string> FiltersList
		{
			get => filtersList;
			set => SetField(ref filtersList, value, () => FiltersList);
		}

		/// Selected filter
		private string selectedFilter;
		public string SelectedFilter
		{
			get => selectedFilter;
			set => SetField(ref selectedFilter, value, () => SelectedFilter);
		}

		/// Are filters visible
		private bool areFiltersVisible = true;
		public bool AreFiltersVisible
		{
			get => areFiltersVisible;
			set => SetField(ref areFiltersVisible, value, () => AreFiltersVisible);
		}

		/// Are groups visible
		private bool areGroupsVisible = true;
		public bool AreGroupsVisible
		{
			get => areGroupsVisible;
			set => SetField(ref areGroupsVisible, value, () => AreGroupsVisible);
		}

		/// Total items
		private int totalItems;
		public int TotalItems
		{
			get => totalItems;
			set => SetField(ref totalItems, value, () => TotalItems);
		}
	}
}
