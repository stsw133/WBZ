using StswExpress.Base;
using StswExpress.Globals;
using System.Collections.ObjectModel;

namespace WBZ.Modules._base
{
	abstract class D_ModuleList<MODULE_MODEL> : D where MODULE_MODEL : class, new()
	{
		/// Instances list
		private ObservableCollection<MODULE_MODEL> instancesList;
		public ObservableCollection<MODULE_MODEL> InstancesList
		{
			get => instancesList;
			set => SetField(ref instancesList, value, () => InstancesList);
		}

		/// Mode
		public Commands.Type Mode { get; set; }

		/// Selecting mode
		public bool SelectingMode => Mode == Commands.Type.SELECT;

		/// Additional window icon
		public string ModeIcon
		{
			get
			{
				if		(Mode == Commands.Type.LIST)	return "/Resources/icon32_list.ico";
				else if	(Mode == Commands.Type.SELECT)	return "/Resources/icon32_select.ico";
				else									return null;
			}
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

		/// Page number
		private int page;
		public int Page
		{
			get => page;
			set => SetField(ref page, value, () => Page);
		}

		/// Total instances number
		private int totalItems;
		public int TotalItems
		{
			get => totalItems;
			set => SetField(ref totalItems, value, () => TotalItems);
		}
	}
}
