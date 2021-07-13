using StswExpress;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using WBZ.Other;
using MODULE_MODEL = WBZ.Models.M_Family;

namespace WBZ.Modules.Families
{
    /// <summary>
    /// Logika interakcji dla klasy FamilyNew.xaml
    /// </summary>
    public partial class FamiliesNew : New
    {
        readonly D_FamiliesNew D = new D_FamiliesNew();

        public FamiliesNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceData = instance;
            D.Mode = mode;
        }

        /// <summary>
		/// Print
		/// </summary>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn != null)
                btn.ContextMenu.IsOpen = true;
        }
        private void btnRodo_Click(object sender, RoutedEventArgs e)
        {
            Prints.Print_RODO(D.InstanceData, SQL.ListContacts(D.Module, D.InstanceData.ID, "default = true").ToList<M_Contact>()?[0]);
        }

        /// <summary>
		/// Tab changed
		/// </summary>
        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
            if (tab?.Name == "tabSources_Distributions")
            {
				if (D.InstanceData.ID != 0 && D.InstanceSources_Distributions == null)
                    D.InstanceSources_Distributions = SQL.ListInstances<M_Distribution>(Config.GetModule(nameof(Modules.Distributions)), $"dp.family={D.InstanceData.ID}");
            }
        }

        /// <summary>
		/// Open: Distribution
		/// </summary>
        private void dgList_Distributions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DtgSourceList_MouseDoubleClick<M_Distribution>(sender, e, Config.GetModule(nameof(Modules.Distributions)));
        }

        /// <summary>
		/// Validation
		/// </summary>
		internal override bool CheckDataValidation()
        {
            if (string.IsNullOrEmpty(D.InstanceData.Declarant))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano osoby zgłaszającej!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceData.Name))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano nazwiska rodziny!") { Owner = this }.ShowDialog();
                return false;
            }
            if (D.InstanceData.Members == 0)
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano liczby osób w rodzinie!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceData.City))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano miasta!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceData.Address))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano adresu!") { Owner = this }.ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(D.InstanceData.Postcode))
            {
                new MsgWin(MsgWin.Types.MsgOnly, MsgWin.Titles.BLOCKADE, "Nie podano kodu pocztowego!") { Owner = this }.ShowDialog();
                return false;
            }

            return true;
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
