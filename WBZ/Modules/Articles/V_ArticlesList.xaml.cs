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
			D.StoresList = SQL.ListValues(Config.Modules.STORES, "codename", "archival=false", D.Mode != Commands.Type.SELECT);
		}

		/// <summary>
		/// Update filters
		/// </summary>
		internal override void UpdateFilters()
		{
			base.UpdateFilters();
			D.FilterSqlString += D.Filters.MainStore.ID > 0 ? $"sa.store={D.Filters.MainStore.ID} and " : string.Empty;
			D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbStore = sender as ComboBox;
			if (cbStore.SelectedValue != null && (int)cbStore.SelectedValue > 0)
				D.Filters.MainStore = SQL.GetInstance<M_Store>(Config.Modules.STORES, (int)cbStore.SelectedValue);
			else
				D.Filters.MainStore = new M_Store();
			cmdRefresh_Executed(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
