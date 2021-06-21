using StswExpress;

namespace WBZ.Login
{
    class D_Versions : D
    {
		/// <summary>
		/// InstancesList
		/// </summary>
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
