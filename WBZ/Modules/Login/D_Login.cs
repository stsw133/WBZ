﻿using StswExpress.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace WBZ.Modules.Login
{
    class D_Login : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Databases list
		private ObservableCollection<M_Database> databases;
		public ObservableCollection<M_Database> Databases
		{
			get => databases;
			set
			{
				databases = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name[4..]);
			}
		}
	}
}
