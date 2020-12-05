using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesList.xaml
	/// </summary>
	public partial class ArticlesList : List
	{
		D_ArticlesList D = new D_ArticlesList();

		public ArticlesList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
			if (D.StoresList.Count > 0)
				cbStore.SelectedIndex = 0;
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"a.archival=false and " : "")
						+ (D.Filters.Group > 0 ? $"g.owner={D.Filters.Group} and " : "")
						+ (SelectedStore?.ID > 0 ? $"sa.store={SelectedStore.ID} and " : "");

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

        /// <summary>
        /// Store - SelectionChanged
        /// </summary>
        public M_Store SelectedStore;
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance<M_Store>(Global.Module.STORES, cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0);
			btnRefresh_Click(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
