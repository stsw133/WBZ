using StswExpress;
using System.Windows;
using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Models;
using WBZ.Modules._base;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
    /// <summary>
    /// Interaction logic for ArticlesList.xaml
    /// </summary>
    public partial class ArticlesList : List
    {
        readonly D_ArticlesList D = new D_ArticlesList();

        public ArticlesList(Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            D.Mode = mode;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        internal override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            D.StoresList = SQL.ComboSource(Config.GetModule(nameof(Stores)), "codename", "is_archival=false", D.Mode != Commands.Type.SELECT);
        }

        /// <summary>
        /// Update filters
        /// </summary>
        internal override void UpdateFilters()
        {
            base.UpdateFilters();
            D.Filter.AutoFilterString += (int?)CmbBoxStoresList?.SelectedValue > 0 ? $" and sa.store={CmbBoxStoresList?.SelectedValue}" : string.Empty;
            if (D.Filter.AutoFilterString.StartsWith(" and "))
                D.Filter.AutoFilterString = D.Filter.AutoFilterString[5..];
        }

        /// <summary>
        /// StoresList - SelectionChanged
        /// </summary>
        private void CmbBoxStoresList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if ((int?)cb?.SelectedValue > 0)
                CmbBoxStoresList.SelectedValue = SQL.GetInstance<M_Store>(Config.GetModule(nameof(Stores)), (int)cb.SelectedValue);
            else
                CmbBoxStoresList.SelectedValue = 0;
            CmdRefresh_Executed(null, null);
        }
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
