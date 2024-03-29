﻿using StswExpress;
using StswExpress.Translate;
using System.Collections.Generic;
using System.Windows.Media;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules._base
{
    abstract class D_ModuleList<MODULE_MODEL> : D where MODULE_MODEL : class, new()
    {
        public D_ModuleList()
        {
            Module = Config.GetModule(GetType().Name[2..^4]);
            Filter = new M_Filter(Module);
        }

        /// Module
        private MV module;
        public MV Module
        {
            get => module;
            set => SetField(ref module, value, () => Module);
        }
        public string Title => TM.Tr(Module.Name + Mode.ToString().Capitalize());

        /// Mode
        public Commands.Type Mode { get; set; }
        public ImageSource ModeIcon => Mode switch
        {
            Commands.Type.LIST => Fn.LoadImage(Properties.Resources.icon32_list),
            Commands.Type.SELECT => Fn.LoadImage(Properties.Resources.icon32_select),
            _ => null
        };

        /// Selected tab
        private int selectedTab = 0;
        public int SelectedTab
        {
            get => selectedTab;
            set => SetField(ref selectedTab, value, () => SelectedTab);
        }

        /// Instances lists
        private List<List<MODULE_MODEL>> instancesLists = new List<List<MODULE_MODEL>>();
        public List<List<MODULE_MODEL>> InstancesLists
        {
            get => instancesLists;
            set => SetField(ref instancesLists, value, () => InstancesLists);
        }

        /// FiltersList
        private List<M_Filter> filtersList = new List<M_Filter>();
        public List<M_Filter> FiltersList
        {
            get => filtersList;
            set => SetField(ref filtersList, value, () => FiltersList);
        }

        /// Filter
        private M_Filter filter;
        public M_Filter Filter
        {
            get => filter;
            set => SetField(ref filter, value, () => Filter);
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

        /// Count items
        private int countItems;
        public int CountItems
        {
            get => countItems;
            set => SetField(ref countItems, value, () => CountItems);
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
