using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WBZ.Modules._base
{
	class D_ModuleList<MODULE_MODEL> : INotifyPropertyChanged where MODULE_MODEL : class, new()
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Instances list
		private ObservableCollection<MODULE_MODEL> instancesList;
		public ObservableCollection<MODULE_MODEL> InstancesList
		{
			get => instancesList;
			set
			{
				instancesList = value;
				NotifyPropertyChanged();
			}
		}
		/// Mode
		public StswExpress.Globals.Commands.Type Mode { get; set; }
		/// Selecting mode
		public bool SelectingMode { get => Mode == StswExpress.Globals.Commands.Type.SELECT; }
		/// SQL filter
		public string FilterSQL { get; set; }
		/// Filters instance
		private MODULE_MODEL filters = new MODULE_MODEL();
		public MODULE_MODEL Filters
		{
			get => filters;
			set
			{
				filters = value;
				NotifyPropertyChanged();
			}
		}
		/// Filters list
		private ObservableCollection<string> filtersList = new ObservableCollection<string>();
		public ObservableCollection<string> FiltersList
		{
			get => filtersList;
			set
			{
				filtersList = value;
				NotifyPropertyChanged();
			}
		}
		/// Selected filter
		private string selectedFilter;
		public string SelectedFilter
		{
			get => selectedFilter;
			set
			{
				selectedFilter = value;
				NotifyPropertyChanged();
			}
		}
		/// Filters visibility
		private bool filtersVisibility = true;
		public bool FiltersVisibility
		{
			get => filtersVisibility;
			set
			{
				filtersVisibility = value;
				NotifyPropertyChanged();
			}
		}
		/// Groups visibility
		private bool groupsVisibility = true;
		public bool GroupsVisibility
		{
			get => groupsVisibility;
			set
			{
				groupsVisibility = value;
				NotifyPropertyChanged();
			}
		}
		/// Page number
		private int page;
		public int Page
		{
			get => page;
			set
			{
				page = value;
				NotifyPropertyChanged();
			}
		}
		/// Total instances number
		private int totalItems;
		public int TotalItems
		{
			get => totalItems;
			set
			{
				totalItems = value;
				NotifyPropertyChanged();
			}
		}
	}
}
