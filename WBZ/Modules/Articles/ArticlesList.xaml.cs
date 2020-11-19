using System.Windows.Controls;
using WBZ.Helpers;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.C_Article;

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

			D.Mode = mode;
			if (D.StoresList.Rows.Count > 0)
				cbStore.SelectedIndex = 0;
		}

		/// Selected
		public MODULE_MODEL Selected;

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		public C_Store SelectedStore;
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance<C_Store>(Global.Module.STORES, cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0);
			btnRefresh_Click(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
