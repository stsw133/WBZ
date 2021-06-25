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
			D.Filter.AutoFilterString += (int?)cbStoresList?.SelectedValue > 0 ? $" and sa.store={cbStoresList?.SelectedValue}" : string.Empty;
			if (D.Filter.AutoFilterString.StartsWith(" and "))
				D.Filter.AutoFilterString = D.Filter.AutoFilterString[5..];
		}

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbStore = sender as ComboBox;
			if ((int)cbStore?.SelectedValue > 0)
				cbStoresList.SelectedValue = SQL.GetInstance<M_Store>(Config.GetModule(nameof(Stores)), (int)cbStore.SelectedValue);
			else
				cbStoresList.SelectedValue = 0;
			cmdRefresh_Executed(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
