using StswExpress;

namespace WBZ.Login
{
    class D_Versions : D
    {
        /// InstancesList
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
