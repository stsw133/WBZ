using StswExpress;

namespace WBZ.Modules.Login
{
    class D_Versions : D
    {
		/// Instances list
		private dynamic instancesList;
		public dynamic InstancesList
		{
			get => instancesList;
			set
			{
				instancesList = value;
				NotifyPropertyChanged();
			}
		}
	}
}
