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
		D_ArticlesList D = new D_ArticlesList();

		public ArticlesList(StswExpress.Globals.Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
		}

		/// <summary>
		/// Loaded
		/// </summary>
		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (D.SelectingMode)
				dgList.SelectionMode = DataGridSelectionMode.Single;
			D.StoresList = SQL.ComboInstances(Global.Module.STORES, "codename", "archival=false", !D.SelectingMode);
		}

		/// <summary>
		/// Update filters
		/// </summary>
		public void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(a.codename,'')) like '%{D.Filters.Codename.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.ean,'')) like '%{D.Filters.EAN.ToLower()}%' and "
						+ (!D.Filters.Archival ? $"a.archival=false and " : string.Empty)
						+ (D.Filters.Group > 0 ? $"exists (select from wbz.groups g where g.instance=a.id and g.owner={D.Filters.Group}) and " : string.Empty)
						+ (D.Filters.MainStore.ID > 0 ? $"sa.store={D.Filters.MainStore.ID} and " : string.Empty);

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

        /// <summary>
        /// Store - SelectionChanged
        /// </summary>
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbStore = sender as ComboBox;
			if (cbStore.SelectedValue != null && (int)cbStore.SelectedValue > 0)
				D.Filters.MainStore = SQL.GetInstance<M_Store>(Global.Module.STORES, (int)cbStore.SelectedValue);
			else
				D.Filters.MainStore = new M_Store();
			cmdRefresh_Executed(null, null);
		}
    }

    public class List : ModuleList<MODULE_MODEL> { }
}
