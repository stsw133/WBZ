﻿using StswExpress.Globals;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WBZ.Modules
{
    class D_Main : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged([CallerMemberName] string name = "none passed")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// Title
		public string Title { get; } = Global.AppDatabase.Name + TranslateMe.TM.Tr("Main", " - okno główne", StswExpress.Globals.Properties.Language);
		/// Want to logout
		public bool WantToLogout { get; set; } = false;
	}
}
